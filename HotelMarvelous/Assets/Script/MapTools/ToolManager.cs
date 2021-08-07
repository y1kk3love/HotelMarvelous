using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;

public class ToolManager : MonoBehaviour
{
    private List<Vector2> selectTilesList = new List<Vector2>();
    private List<string[]> furnitureDataList = new List<string[]>();

    private Coroutine errorcoroutine = null;

    private Camera bpCamera;                                             //청사진 카메라

    private GameObject obRoomGridParent;                                 //생성된 17x17 그리드의 부모

    private GameObject obRoomPick;                                    //17x17 그리드 리소스
    private GameObject obFloor;                                         //타일 바닥 리소스
    private GameObject obBlockWall;                                    //막힌 벽 리소스
    private GameObject obDoorWall;                                    //문있는 벽 리소스
    private GameObject obDragBox;                                     //드래그 박스 리소스   
    private GameObject obMonPosCircle;                               //몬스터 스폰포인트를 표시할 리소스
    private GameObject obfurniture;

    private GameObject obCurDragBox = null;                         //현재 생성된 드래그 박스
    private GameObject obCurDragSelectBox = null;                  //현재 생성된 드래스 선택 박스들을 넣은 빈오브젝트
    private GameObject obcurMonCircle;                                   //현재 선택된 몬스터 스폰포인트
    private GameObject obCurFurnPreviewModel = null;
    private GameObject obCurFurnMouseModel = null;

    private GameObject obFurniturePanel;
    private GameObject obWallPanel;                                    //벽을 관리하는 인터페이스
    private GameObject obMonsterPanel;                               //몬스터 스폰포인트를 관리하는 인터페이스

    private Text textTilePos;                                    //선택된 타일의 좌표, 방인덱스등이 나오는 텍스트
    private Text textMakeTile;
    private Text textDelTile;
    private Text textFurnitureDir;
    private Text textMonsterInfo;

    private Text textFloorPattern;
    private Text textNowfloor;

    private InputField inputFRoomIndex;                          //방의 인덱스를 입력받는 인풋필드
    private InputField inputFDataFileName;

    private Dropdown ddTopWall;                                  //타일 벽을 설정하기 위한 드롭다운들
    private Dropdown ddRightWall;
    private Dropdown ddBottomWall;
    private Dropdown ddLeftWall;

    private Dropdown ddMonType;                                  //몬스터 타입을 받아올 드롭다운
    private Dropdown ddFurnType;                                 //가구 타입을 받아올 드롭다운
    private Dropdown ddNewFloor;

    private TileInfo[,] mapBoardArr = new TileInfo[51, 51];      //타일의 정보를 담은 클래스를 가진 배열
    private TileData[,] mapDataArr = new TileData[51, 51];
    private TileInfo curTile = null;                             //현재 선택된 타일의 정보

    private TOOLEDITUI editMode = 0;
    private ROTATION furnitureDir;

    private Vector2? dragGridStartPos = null;                    //카메라 이동을 시작한 위치, 기준점
    private Vector2 dragBPCurPos;                                //현재 마우스의 위치
    private Vector2 dragBPCamPos;                                //카메라 이동을 시작했을때 카메라의 위치

    private Vector2 dragBoxStartPos;                             //드래그 박스를 위한 좌표
    private Vector2 dragBoxCurPos;

    private int curTileX, curTileY;                              //현재 선택한 타일의 실제 좌표 18/1사이즈
    private int[] DataIndex = new int[3];

    private float cameraWheelSpeed = 20.0f;                      //카메라 줌 스피드
    private float minCamZoom = 5.0f;                             //카메라 줌 최소사이즈
    private float maxCamZoom = 450.0f;                           //카메라 줌 최대사이즈

    private string editingfilename = null;

    private bool isTileSelect = false;                           //타일이 선택되었는지 확인
    private bool isWallChanging = false;                         //UI를 통해 벽을 수정중인지 확인
    private bool isLoadingData = false;
    private bool isEditData = false;

    void Start()
    {
        StartReset();
        CreateEmpty();
        ResetDropDowns();
        UIOFF();

        LoadFurnitureCSVData();
        LoadDataIndex();

        ReSetDataIndex();
    }

    void Update()
    {
        //UI에 마우스가 올라가 있으면 작동하지 않게함
        StopUpdateOnUI();

        //카메라 이동과 줌
        CameraDragMove();
        CameraWheelZoom();

        //타일 선택과 인덱스 단축키
        FloorPick();
        ObjectPick();
        IndexUpper();
        RoomTypeChanger();

        //타일생성 삭제 등 관리
        MapControl();
        ControlTileWall(curTile);
    }

    #region [CameraMove]

    //카메라 우클릭 드래그 이동
    private void CameraDragMove()
    {
        dragBPCurPos = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            dragGridStartPos = Input.mousePosition;
            dragBPCamPos = bpCamera.transform.position;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            dragGridStartPos = null;
        }

        if (Input.GetKey(KeyCode.Mouse1) && dragGridStartPos != null)
        {
            Vector2 dir = (Vector2)dragGridStartPos - dragBPCurPos;
            Vector2 _pos = dragBPCamPos +dir * (bpCamera.orthographicSize / (minCamZoom + maxCamZoom) / 2);
            bpCamera.transform.position = new Vector3(_pos.x, _pos.y, -6);
        }
    }

    //카메라 휠 줌인 아웃
    private void CameraWheelZoom()           
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * cameraWheelSpeed * (bpCamera.orthographicSize / -15.0f);

        if (bpCamera.orthographicSize <= minCamZoom && scroll < 0)
        {
            bpCamera.orthographicSize = minCamZoom;
        }
        else if(bpCamera.orthographicSize >= maxCamZoom && scroll > 0)
        {
            bpCamera.orthographicSize = maxCamZoom;
        }
        else
        {
            bpCamera.orthographicSize += scroll;
        }
    }

    #endregion

    #region [MapSelect]

    private void StopUpdateOnUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
    }

    private void ObjectPick()
    {
        Ray ray = bpCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)] != null)
            {
                if (editMode == TOOLEDITUI.MONSTERMODE)
                {
                    float x = BPMonPosParse(hit.point.x) + 0.5f;
                    float y = BPMonPosParse(hit.point.y) + 0.5f;

                    if (!mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].monSpawnInfoDic.TryGetValue(new Vector2(x, y), out MONSTERTYPE _dmtype))
                    {
                        textMonsterInfo.text = string.Format("(X : {0} // Y : {1}) {2}", x, y, _dmtype);
                    }
                    else
                    {
                        MONSTERTYPE _type = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].monSpawnInfoDic[new Vector2(x, y)];
                        textMonsterInfo.text = string.Format("(X : {0} // Y : {1}) {2}", x, y, _type);
                    }
                }
                else if (editMode == TOOLEDITUI.FURNITUREMODE)
                {
                    //위와 같은 실시간 가구 정보 확인 기능 구현예정

                    if (obCurFurnMouseModel != null)
                    {
                        Vector2 _pos = hit.point;

                        obCurFurnMouseModel.transform.position = _pos;
                    }
                }
            }
            else
            {
                if (obCurFurnMouseModel != null)
                {
                    obCurFurnMouseModel.transform.position = new Vector2(-1000, -1000);
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)] != null)
                {
                    if (editMode == TOOLEDITUI.MONSTERMODE)
                    {
                        if (ddMonType.value == 0)
                        {
                            ErrorMessage("몬스터의 타입이 설정되지 않았습니다!!!");
                        }
                        else
                        {
                            float x = BPMonPosParse(hit.point.x);
                            float y = BPMonPosParse(hit.point.y);

                            MONSTERTYPE _monType = (MONSTERTYPE)ddMonType.value;

                            if (!mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].monSpawnInfoDic.TryGetValue(new Vector2(x + 0.5f, y + 0.5f), out MONSTERTYPE _dmtype))
                            {
                                obcurMonCircle = Instantiate(obMonPosCircle, new Vector2(x, y), Quaternion.identity);
                                obcurMonCircle.transform.parent = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].obTile.transform;
                                obcurMonCircle.name = string.Format("{0} // {1} MonPos", x + 0.5f, y + 0.5f);
                                mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].monSpawnInfoDic.Add(new Vector2(x + 0.5f, y + 0.5f), _monType);
                            }
                            else
                            {
                                Destroy(GameObject.Find(string.Format("{0} // {1} MonPos", x + 0.5f, y + 0.5f)));
                                mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].monSpawnInfoDic.Remove(new Vector2(x + 0.5f, y + 0.5f));
                            }
                        }
                    }
                    else if (editMode == TOOLEDITUI.FURNITUREMODE)
                    {
                        if (ddFurnType.value == 0)
                        {
                            ErrorMessage("가구의 타입이 설정되지 않았습니다!!!");
                        }
                        else
                        {
                            float x = BPMonPosParse(hit.point.x);
                            float y = BPMonPosParse(hit.point.y);

                            if (!mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].FurnitureInfoDic.TryGetValue(new Vector2(x + 0.5f, y + 0.5f), out FurnitureInfo _furinfo))
                            {
                                string[] _data = furnitureDataList[ddFurnType.value];

                                FurnitureInfo _info = new FurnitureInfo();

                                _info.name = _data[(int)FURNITUREDATA.NAME];
                                _info.pos = new Vector2(x, y);
                                _info.dir = furnitureDir;

                                mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].FurnitureInfoDic.Add(new Vector2(x + 0.5f, y + 0.5f), _info);

                                int _rotate = (int)furnitureDir * 90;
                                GameObject _furniture = Instantiate(obfurniture, new Vector2(x, y), Quaternion.Euler(new Vector3(-90 + _rotate, 90, -90)));
                                _furniture.transform.parent = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].obTile.transform;
                                _furniture.name = string.Format("{0} // {1} {2}", x + 0.5f, y + 0.5f, _info.name);
                            }
                            else
                            {
                                string _name = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].FurnitureInfoDic[new Vector2(x + 0.5f, y + 0.5f)].name;

                                Destroy(GameObject.Find(string.Format("{0} // {1} {2}", x + 0.5f, y + 0.5f, _name)));
                                mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].FurnitureInfoDic.Remove(new Vector2(x + 0.5f, y + 0.5f));
                            }
                        }
                    }
                }
            }
        }
    }
    
    private void FloorPick()
    {
        Ray ray = bpCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Destroy(obCurDragBox);
                return;
            }

            curTileX = EditPosParse(hit.point.x);
            curTileY = EditPosParse(hit.point.y);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ROOMTYPE _type = ROOMTYPE.EMPTY;

                string _index;

                dragBoxStartPos = hit.point;

                isWallChanging = false;

                if (mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)] != null)
                {
                    _index = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].roomIndex.ToString();
                    _type = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].roomType;
                }
                else
                {
                    _index = "-1";
                }

                if (!isTileSelect)
                {
                    isTileSelect = true;

                    textTilePos.text = string.Format("Tile X : {0} // Y : {1}\nIndex {2} // Type {3}", (curTileX / 18), (curTileY / 18), _index, _type);
                }
                else
                {
                    textTilePos.text = string.Format("Tile X : {0} // Y : {1}\nIndex {2} // Type {3}", (curTileX / 18), (curTileY / 18), _index, _type);
                    curTile = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)];

                    if (mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)] != null)
                    {
                        inputFRoomIndex.text = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].roomIndex.ToString();

                        for (int i = 0; i < 4; i++)
                        {
                            switch (i)
                            {
                                case (int)DIRECTION.TOP:
                                    ddTopWall.value = (byte)curTile.doorArr[i];
                                    break;
                                case (int)DIRECTION.RIGHT:
                                    ddRightWall.value = (byte)curTile.doorArr[i];
                                    break;
                                case (int)DIRECTION.BOTTOM:
                                    ddBottomWall.value = (byte)curTile.doorArr[i];
                                    break;
                                case (int)DIRECTION.LEFT:
                                    ddLeftWall.value = (byte)curTile.doorArr[i];
                                    break;
                            }
                        }
                    }
                }
            }
        }

        if (editMode != TOOLEDITUI.TILEMODE)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0) && editMode == TOOLEDITUI.TILEMODE)
        {
            if (obCurDragBox == null)
            {
                obCurDragBox = Instantiate(obDragBox, dragBoxStartPos, Quaternion.identity);
            }

            dragBoxCurPos = hit.point;

            obCurDragBox.transform.position = ((dragBoxStartPos + dragBoxCurPos) / 2);
            obCurDragBox.transform.localScale = new Vector2(Mathf.Abs(dragBoxStartPos.x - dragBoxCurPos.x), Mathf.Abs(dragBoxStartPos.y - dragBoxCurPos.y));
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            selectTilesList.Clear();

            Destroy(obCurDragBox);
            Destroy(obCurDragSelectBox);

            obCurDragBox = null;
            obCurDragSelectBox = null;

            obCurDragSelectBox = new GameObject("Select Boxes");

            int _stx = Mathf.RoundToInt(dragBoxStartPos.x / 18);
            int _enx = Mathf.RoundToInt(dragBoxCurPos.x / 18);
            int _sty = Mathf.RoundToInt(dragBoxStartPos.y / 18);
            int _eny = Mathf.RoundToInt(dragBoxCurPos.y / 18);

            if (_stx > _enx)
            {
                _stx = Mathf.RoundToInt(dragBoxCurPos.x / 18);
                _enx = Mathf.RoundToInt(dragBoxStartPos.x / 18);
            }

            if (_sty > _eny)
            {
                _sty = Mathf.RoundToInt(dragBoxCurPos.y / 18);
                _eny = Mathf.RoundToInt(dragBoxStartPos.y / 18);
            }

            for (int x = _stx; x < _enx + 1; x++)
            {
                for (int y = _sty; y < _eny + 1; y++)
                {
                    selectTilesList.Add(new Vector2(x * 18, y * 18));
                    GameObject _box = Instantiate(obDragBox, new Vector2(x * 18, y * 18), Quaternion.identity);
                    _box.transform.parent = obCurDragSelectBox.transform;
                }
            }
        }
    }

    private void IndexUpper()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (editMode == TOOLEDITUI.TILEMODE)
            {
                if (inputFRoomIndex.text == "")
                {
                    inputFRoomIndex.text = "1";
                }
                else
                {
                    inputFRoomIndex.text = (byte.Parse(inputFRoomIndex.text) + 1).ToString();
                }
            }
            else if (editMode == TOOLEDITUI.FURNITUREMODE)
            {
                if (ddFurnType.value < furnitureDataList.Count - 1)
                {
                    if (obCurFurnPreviewModel != null)
                    {
                        Destroy(obCurFurnPreviewModel);
                    }

                    if (obCurFurnMouseModel != null)
                    {
                        Destroy(obCurFurnMouseModel);
                    }

                    ddFurnType.value++;

                    string _modelname = furnitureDataList[ddFurnType.value][(int)FURNITUREDATA.NAME];
                    obfurniture = Resources.Load("MapTools/Furniture/" + _modelname) as GameObject;
                    obCurFurnPreviewModel = Instantiate(obfurniture, new Vector2(1000, 1000), Quaternion.identity);
                    obCurFurnPreviewModel.name = "FurniturePreview";

                    int _rotate = (int)furnitureDir * 90;
                    obCurFurnMouseModel = Instantiate(obfurniture, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(-90 + _rotate, 90, -90)));
                    obCurFurnMouseModel.name = "FurniturPointer";
                }
                else
                {
                    if (obCurFurnPreviewModel != null)
                    {
                        Destroy(obCurFurnPreviewModel);
                        obCurFurnPreviewModel = null;
                    }

                    if(obCurFurnMouseModel != null)
                    {
                        Destroy(obCurFurnMouseModel);
                        obCurFurnMouseModel = null;
                    }

                    ddFurnType.value = 0;
                }
            }
            else
            {
                if (ddMonType.value < System.Enum.GetValues(typeof(MONSTERTYPE)).Length - 1)
                {
                    ddMonType.value++;
                }
                else 
                {
                    ddMonType.value = 0;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (editMode == TOOLEDITUI.TILEMODE)
            {
                if (inputFRoomIndex.text == "")
                {
                    inputFRoomIndex.text = "0";
                }
                else
                {
                    if (byte.Parse(inputFRoomIndex.text) > 0)
                    {
                        inputFRoomIndex.text = (byte.Parse(inputFRoomIndex.text) - 1).ToString();
                    }
                }
            }
            else if (editMode == TOOLEDITUI.FURNITUREMODE)
            {
                if (ddFurnType.value > 0)
                {
                    if (obCurFurnPreviewModel != null)
                    {
                        Destroy(obCurFurnPreviewModel);
                    }

                    if (obCurFurnMouseModel != null)
                    {
                        Destroy(obCurFurnMouseModel);
                    }

                    ddFurnType.value--;

                    if(ddFurnType.value != 0)
                    {
                        string _modelname = furnitureDataList[ddFurnType.value][(int)FURNITUREDATA.NAME];
                        obfurniture = Resources.Load("MapTools/Furniture/" + _modelname) as GameObject;
                        obCurFurnPreviewModel = Instantiate(obfurniture, new Vector2(1000, 1000), Quaternion.identity);

                        int _rotate = ((int)furnitureDir + 1) * 90;
                        obCurFurnMouseModel = Instantiate(obfurniture, new Vector3(0, 0, 0), Quaternion.Euler(-90 + _rotate, 90, -90));
                    }
                }
                else
                {
                    if (obCurFurnPreviewModel != null)
                    {
                        Destroy(obCurFurnPreviewModel);
                    }

                    if (obCurFurnMouseModel != null)
                    {
                        Destroy(obCurFurnMouseModel);
                    }

                    ddFurnType.value = furnitureDataList.Count - 1;

                    string _modelname = furnitureDataList[ddFurnType.value][1];
                    GameObject _model = Resources.Load("MapTools/Furniture/" + _modelname) as GameObject;
                    obCurFurnPreviewModel = Instantiate(_model, new Vector2(1000, 1000), Quaternion.identity);
                }
            }
            else
            {
                if (ddMonType.value > 0)
                {
                    ddMonType.value--;
                }
                else
                {
                    ddMonType.value = System.Enum.GetValues(typeof(MONSTERTYPE)).Length - 1;
                }
            }
        }       
    }

    private void RoomTypeChanger()
    {
        if(curTile != null)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                TypeChange(ROOMTYPE.EMPTY);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                TypeChange(ROOMTYPE.HALLWAY);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                TypeChange(ROOMTYPE.GUEST);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                TypeChange(ROOMTYPE.NPC);
            }
        }
    }

    private void TypeChange(ROOMTYPE _type)
    {
        Color _color = Color.white;

        switch ((int)_type)
        {
            case (int)ROOMTYPE.HALLWAY:
                _color = Color.red;
                break;
            case (int)ROOMTYPE.GUEST:
                _color = Color.yellow;
                break;
            case (int)ROOMTYPE.NPC:
                _color = Color.green;
                break;
        }

        foreach (Vector2 _pos in selectTilesList)
        {
            int _x = (int)_pos.x;
            int _y = (int)_pos.y;

            if (mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] != null)
            {
                TileInfo _info = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)];

                _info.roomType = _type;
                _info.obTile.transform.Find("Floor(Clone)").GetComponent<SpriteRenderer>().color = _color;
            }
        }

        textTilePos.text = string.Format("Tile X : {0} // Y : {1}\nIndex {2} // Type {3}", (curTileX / 18), (curTileY / 18), curTile.roomIndex, _type);
    }

    #endregion

    #region [MapControl]

    private void MapControl()
    {
        //타일 생성 단축키
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (editMode == TOOLEDITUI.TILEMODE)
            {
                CreateMap();
            }
            else if (editMode == TOOLEDITUI.FURNITUREMODE)
            {
                if (furnitureDir > 0)
                {
                    furnitureDir--;
                }
                else
                {
                    furnitureDir = (ROTATION)3;
                }
                
                textFurnitureDir.text = furnitureDir.ToString();

                if(obCurFurnMouseModel != null)
                {
                    int _rotate = (int)furnitureDir * 90;
                    obCurFurnMouseModel.transform.rotation = Quaternion.Euler(-90 + _rotate, 90, -90);
                }
            }  
        }

        //타일 삭제 단축키
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (editMode == TOOLEDITUI.TILEMODE)
            {
                DeleteMap();
            }
            else if (editMode == TOOLEDITUI.FURNITUREMODE)
            {
                if (furnitureDir < (ROTATION)3)
                {
                    furnitureDir++;
                }
                else
                {
                    furnitureDir = 0;
                }

                textFurnitureDir.text = furnitureDir.ToString();

                int _rotate = (int)furnitureDir * 90;
                obCurFurnMouseModel.transform.rotation = Quaternion.Euler(-90 + _rotate, 90, -90);
            }
        }
    }

    public void CreateMap()
    {
        foreach (Vector2 _pos in selectTilesList)
        {
            int _x = (int)_pos.x;
            int _y = (int)_pos.y;

            if (mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] == null || isLoadingData)
            {
                GameObject emptytile = new GameObject(string.Format("Tile/{0},{1}", (_x / 18), (_y / 18)));
                emptytile.transform.position = new Vector3(_x, _y, 9);

                GameObject floor = Instantiate(obFloor, new Vector3(_x, _y, 9), Quaternion.identity);
                floor.transform.parent = emptytile.transform;

                TextMesh _roomnum = floor.transform.Find("RoomNum").GetComponent<TextMesh>();

                if (!isLoadingData)
                {
                    mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] = new TileInfo();
                }
                
                mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].obTile = emptytile;
                mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].position = new Vector3(_x, _y, 9);

                if (inputFRoomIndex.text == "" && !isLoadingData)        //최초 실행일때 인덱스값 1 설정
                {
                    inputFRoomIndex.text = "1";
                    mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].roomIndex = 1;
                }
                else if (isLoadingData)
                {
                    inputFRoomIndex.text = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].roomIndex.ToString();
                }
                else
                {
                    mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].roomIndex = byte.Parse(inputFRoomIndex.text);
                }

                _roomnum.text = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].roomIndex.ToString();
                textTilePos.text = string.Format("Tile X : {0} // Y : {1}\nIndex {2} // Type {3}", (_x / 18), (_y / 18), mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].roomIndex, ROOMTYPE.EMPTY);

                curTile = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)];
            }
        }

        foreach (Vector2 _pos in selectTilesList)
        {
            BuildWall(mapBoardArr[BoardPosParse((int)_pos.x), BoardPosParse((int)_pos.y)]);       //생성된 타일 위에 벽 생성
        }
    }

    public void DeleteALLMap()
    {
        foreach (TileInfo _info in mapBoardArr)
        {
            if(_info != null)
            {
                int _x = (int)_info.position.x;
                int _y = (int)_info.position.y;

                //맵을 오브젝트 삭제 후 배열에서도 비우기
                if (mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] != null)
                {
                    Destroy(mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].obTile);

                    mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] = null;
                }
            } 
        }
    }

    public void DeleteMap()
    {
        foreach (Vector2 _pos in selectTilesList)
        {
            int _x = (int)_pos.x;
            int _y = (int)_pos.y;

            //맵을 오브젝트 삭제 후 배열에서도 비우기
            if (mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] != null)
            {
                Destroy(mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].obTile);

                mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] = null;

                for (int i = 0; i < 4; i++)
                {
                    switch (i)
                    {
                        case (int)DIRECTION.TOP:
                            TileInfo _TT = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y + 18)];

                            if (_TT != null)
                            {
                                _TT.doorArr[(byte)DIRECTION.BOTTOM] = WALLSTATE.BLOCK;

                                BuildWall(_TT);
                            }
                            break;
                        case (int)DIRECTION.RIGHT:
                            TileInfo _RT = mapBoardArr[BoardPosParse(_x + 18), BoardPosParse(_y)];

                            if (_RT != null)
                            {
                                _RT.doorArr[(byte)DIRECTION.LEFT] = WALLSTATE.BLOCK;

                                BuildWall(_RT);
                            }
                            break;
                        case (int)DIRECTION.BOTTOM:
                            TileInfo _BT = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y - 18)];

                            if (_BT != null)
                            {
                                _BT.doorArr[(byte)DIRECTION.TOP] = WALLSTATE.BLOCK;

                                BuildWall(_BT);
                            }
                            break;
                        case (int)DIRECTION.LEFT:
                            TileInfo _LT = mapBoardArr[BoardPosParse(_x - 18), BoardPosParse(_y)];

                            if (_LT != null)
                            {
                                _LT.doorArr[(byte)DIRECTION.RIGHT] = WALLSTATE.BLOCK;

                                BuildWall(_LT);
                            }
                            break;
                    }
                }
            }
        }
    }

    private void ControlTileWall(TileInfo _tileinfo)
    {
        if(_tileinfo != null)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                byte _dir = (byte)_tileinfo.doorArr[(byte)DIRECTION.TOP];

                if (_dir > 1)
                {
                    _dir = 0;
                }
                else
                {
                    _dir++;
                }

                _tileinfo.doorArr[(byte)DIRECTION.TOP] = (WALLSTATE)_dir;
                //dbTopWall.value = (byte)_tileinfo.doorArr[(byte)DIRECTION.TOP];               
                BuildWall(_tileinfo);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                byte _dir = (byte)_tileinfo.doorArr[(byte)DIRECTION.RIGHT];

                if (_dir > 1)
                {
                    _dir = 0;
                }
                else
                {
                    _dir++;
                }

                _tileinfo.doorArr[(byte)DIRECTION.RIGHT] = (WALLSTATE)_dir;
                //dbRightWall.value = (byte)_tileinfo.doorArr[(byte)DIRECTION.RIGHT];
                BuildWall(_tileinfo);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                byte _dir = (byte)_tileinfo.doorArr[(byte)DIRECTION.BOTTOM];

                if (_dir > 1)
                {
                    _dir = 0;
                }
                else
                {
                    _dir++;
                }

                _tileinfo.doorArr[(byte)DIRECTION.BOTTOM] = (WALLSTATE)_dir;
                //dbBottomWall.value = (byte)_tileinfo.doorArr[(byte)DIRECTION.BOTTOM];
                BuildWall(_tileinfo);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                byte _dir = (byte)_tileinfo.doorArr[(byte)DIRECTION.LEFT];

                if (_dir > 1)
                {
                    _dir = 0;
                }
                else
                {
                    _dir++;
                }

                _tileinfo.doorArr[(byte)DIRECTION.LEFT] = (WALLSTATE)_dir;
                //dbLeftWall.value = (byte)_tileinfo.doorArr[(byte)DIRECTION.LEFT];
                BuildWall(_tileinfo);
            }
        }
    }

    private void BuildWall(TileInfo _tileinfo)
    {
        if (_tileinfo.obTile.transform.Find("Walls") != null)
        {
            Destroy(_tileinfo.obTile.transform.Find("Walls").gameObject);
        }

        TileInfo _aroundinfo = null;    //상하좌우 주변 타일의 정보

        GameObject emptywall;           //벽들을 자식으로 가지고 있을 빈오브젝트

        emptywall = new GameObject("Walls");
        emptywall.transform.parent = _tileinfo.obTile.transform;

        for (int i = 0; i < 4; i++)
        {
            GameObject _wall = null;

            Vector2 _pos = _tileinfo.obTile.transform.position;
            int _x = (int)_pos.x;
            int _y = (int)_pos.y;

            switch (_tileinfo.doorArr[i])       //배열에서 벽 확인
            {
                case WALLSTATE.BLOCK:
                    _wall = obBlockWall;
                    break;
                case WALLSTATE.EMPTY:
                    break;
                case WALLSTATE.DOOR:
                    _wall = obDoorWall;
                    break;
            }

            switch (i)
            {
                case (int)DIRECTION.TOP:

                    if (_wall != null)
                    {
                        _aroundinfo = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y + 18)];

                        if (_aroundinfo != null && _tileinfo.roomIndex == _aroundinfo.roomIndex)    //이웃에 같은 인덱스의 타일이 있으면 그 공간을 비우고 이웃의 벽도 허물어준다.
                        {
                            _tileinfo.doorArr[i] = WALLSTATE.EMPTY;

                            _aroundinfo.doorArr[(byte)DIRECTION.BOTTOM] = WALLSTATE.EMPTY;

                            Transform _neerTW = _aroundinfo.obTile.transform.Find("Walls");

                            if (_neerTW != null && _neerTW.Find("BottomWall") != null)
                            {
                                Destroy(_neerTW.Find("BottomWall").gameObject);
                            }
                        }
                        else
                        {
                            GameObject topWall = Instantiate(_wall, new Vector2(_x, _y + 8.5f), Quaternion.Euler(0, 0, 90));
                            topWall.name = "TopWall";
                            topWall.transform.parent = emptywall.transform;
                        }

                        ddTopWall.value = (byte)_tileinfo.doorArr[i];
                    }
                    else
                    {
                        ddTopWall.value = 1;
                    }

                    break;
                case (int)DIRECTION.RIGHT:

                    if (_wall != null)
                    {
                        _aroundinfo = mapBoardArr[BoardPosParse(_x + 18), BoardPosParse(_y)];

                        if (_aroundinfo != null && _tileinfo.roomIndex == _aroundinfo.roomIndex)
                        {
                            _tileinfo.doorArr[i] = WALLSTATE.EMPTY;

                            _aroundinfo.doorArr[(byte)DIRECTION.LEFT] = WALLSTATE.EMPTY;

                            Transform _neerLW = _aroundinfo.obTile.transform.Find("Walls");

                            if (_neerLW != null && _neerLW.Find("LeftWall") != null)
                            {
                                Destroy(_neerLW.Find("LeftWall").gameObject);
                            }
                        }
                        else
                        {
                            GameObject rightWall = Instantiate(_wall, new Vector2(_x + 8.5f, _y), Quaternion.Euler(0, 0, 0));
                            rightWall.name = "RightWall";
                            rightWall.transform.parent = emptywall.transform;
                        }

                        ddRightWall.value = (byte)_tileinfo.doorArr[i];
                    }
                    else
                    {
                        ddRightWall.value = 1;
                    }
                    break;
                case (int)DIRECTION.BOTTOM:

                    if (_wall != null)
                    {
                        _aroundinfo = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y - 18)];

                        if (_aroundinfo != null && _tileinfo.roomIndex == _aroundinfo.roomIndex)
                        {
                            _tileinfo.doorArr[i] = WALLSTATE.EMPTY;

                            _aroundinfo.doorArr[(byte)DIRECTION.TOP] = WALLSTATE.EMPTY;

                            Transform _neerBW = _aroundinfo.obTile.transform.Find("Walls");

                            if (_neerBW != null && _neerBW.Find("TopWall") != null)
                            {
                                Destroy(_neerBW.Find("TopWall").gameObject);
                            }
                        }
                        else
                        {
                            GameObject bottomWall = Instantiate(_wall, new Vector2(_x, _y - 8.5f), Quaternion.Euler(0, 0, -90));
                            bottomWall.name = "BottomWall";
                            bottomWall.transform.parent = emptywall.transform;
                        }

                        ddBottomWall.value = (byte)_tileinfo.doorArr[i];
                    }
                    else
                    {
                        ddBottomWall.value = 1;
                    }

                    break;
                case (int)DIRECTION.LEFT:

                    if (_wall != null)
                    {
                        _aroundinfo = mapBoardArr[BoardPosParse(_x - 18), BoardPosParse(_y)];

                        if (_aroundinfo != null && _tileinfo.roomIndex == _aroundinfo.roomIndex)
                        {
                            _tileinfo.doorArr[i] = WALLSTATE.EMPTY;

                            _aroundinfo.doorArr[(byte)DIRECTION.RIGHT] = WALLSTATE.EMPTY;

                            Transform _neerBW = _aroundinfo.obTile.transform.Find("Walls");

                            if (_neerBW != null && _neerBW.Find("RightWall") != null)
                            {
                                Destroy(_neerBW.Find("RightWall").gameObject);
                            }
                        }
                        else
                        {
                            GameObject leftWall = Instantiate(_wall, new Vector2(_x - 8.5f, _y), Quaternion.Euler(0, 0, -180));
                            leftWall.name = "LeftWall";
                            leftWall.transform.parent = emptywall.transform;
                        }

                        ddLeftWall.value = (byte)_tileinfo.doorArr[i];
                    }
                    else
                    {
                        ddLeftWall.value = 1;
                    }

                    break;
            }
        }
    }

    public void OnDropdownnEvent(int index)
    {
        if (curTile != null && isWallChanging)
        {
            curTile.doorArr[(byte)DIRECTION.TOP] = (WALLSTATE)ddTopWall.value;
            curTile.doorArr[(byte)DIRECTION.RIGHT] = (WALLSTATE)ddRightWall.value;
            curTile.doorArr[(byte)DIRECTION.BOTTOM] = (WALLSTATE)ddBottomWall.value;
            curTile.doorArr[(byte)DIRECTION.LEFT] = (WALLSTATE)ddLeftWall.value;

            BuildWall(curTile);
            isWallChanging = false;
        }
    }

    public void OnDropdownWallChange()
    {
        isWallChanging = true;
    }


    public void OnClickButtonGridMode()
    {
        Text _buttontext = GameObject.Find("TextGridMode").GetComponent<Text>();

        if (editMode == TOOLEDITUI.TILEMODE)
        {
            editMode = TOOLEDITUI.MONSTERMODE;
            _buttontext.text = "몬스터 설치중";
            textMakeTile.text = "배치하기 (Q)";
            textDelTile.text = "삭제하기 (E)";

            obMonsterPanel.SetActive(true);
            obWallPanel.SetActive(false);

            for (int _x = -25; _x < 25; _x++)
            {
                for (int _y = -25; _y < 25; _y++)
                {
                    if(mapBoardArr[BoardPosParse(_x * 18), BoardPosParse(_y * 18)] != null)
                    {
                        GameObject _grid = Instantiate(obRoomPick, new Vector2(_x * 18, _y * 18), Quaternion.identity);
                        _grid.transform.parent = obRoomGridParent.transform;
                    }
                }
            }
        }
        else if (editMode == TOOLEDITUI.MONSTERMODE)
        {
            editMode = TOOLEDITUI.FURNITUREMODE;
            _buttontext.text = "가구 설치중";
            textMakeTile.text = "좌로 회전 (Q)";
            textDelTile.text = "우로 회전 (E)";

            obMonsterPanel.SetActive(false);
            obFurniturePanel.SetActive(true);
        }
        else
        {
            editMode = TOOLEDITUI.TILEMODE;
            _buttontext.text = "방 생성중";
            textMakeTile.text = "타일 생성 (Q)";
            textDelTile.text = "타일 삭제 (E)";

            obFurniturePanel.SetActive(false);
            obWallPanel.SetActive(true);

            Destroy(obRoomGridParent);
            obRoomGridParent = new GameObject("Grids");
        }
    }
    #endregion

    #region [Calculator]

    private float BPMonPosParse(float _hitpoint)
    {
        float i = _hitpoint % 1;

        if (i > 0 && i <= 1)
        {
            return (float)System.Math.Truncate(_hitpoint) + 0.5f;
        }
        else if ( i < 0 && i >= -1)
        {
            return (float)System.Math.Truncate(_hitpoint) - 0.5f;
        }
        else
        {
            return (float)System.Math.Truncate(_hitpoint);
        }
    }

    //청사진을 위한 변환
    private int EditPosParse(float _pos)
    {
        float i = _pos % 18;

        if(i >= 9)
        {
            return (int)System.Math.Truncate((_pos / 9) + 1) * 9;
        }
        else if(i <= -9)
        {
            return (int)System.Math.Truncate((_pos / 9) - 1) * 9;
        }
        else
        {
            return (int)System.Math.Truncate(_pos / 9) * 9;
        }
    }

    //배열에서의 데이터 관리를 위한 변환
    private byte BoardPosParse(int _pos)
    {
        return (byte)(_pos / 18 + 25);
    }

    #endregion

    #region [DataReset]

    private void StartReset()
    {
        bpCamera = Camera.main.GetComponent<Camera>();

        obRoomPick = Resources.Load("MapTools/Prefab/RoomPick") as GameObject;
        obFloor = Resources.Load("MapTools/Prefab/Floor") as GameObject;
        obBlockWall = Resources.Load("MapTools/Prefab/Wall") as GameObject;
        obDoorWall = Resources.Load("MapTools/Prefab/Door") as GameObject;
        obDragBox = Resources.Load("MapTools/Prefab/BoxSelect") as GameObject;
        obMonPosCircle = Resources.Load("MapTools/Prefab/MonsterPos") as GameObject;

        textMonsterInfo = GameObject.Find("TextMonsterInfo").GetComponent<Text>();
        textFurnitureDir = GameObject.Find("FurnitureDirText").GetComponent<Text>();
        textTilePos = GameObject.Find("TextTilePos").GetComponent<Text>();
        textMakeTile = GameObject.Find("TextMakeTile").GetComponent<Text>();
        textDelTile = GameObject.Find("TextDelTile").GetComponent<Text>();

        textFloorPattern = GameObject.Find("TextNumOfPrefab").GetComponent<Text>();
        textNowfloor = GameObject.Find("TextNumOfFloor").GetComponent<Text>();

        inputFRoomIndex = GameObject.Find("InputFieldRoomNum").GetComponent<InputField>();
        inputFDataFileName = GameObject.Find("InputFieldFileName").GetComponent<InputField>();

        obFurniturePanel = GameObject.Find("FurniturePanel");
        obWallPanel = GameObject.Find("WallPanel");
        obMonsterPanel = GameObject.Find("MonsterPanel");
    }

    private void CreateEmpty()
    {
        obRoomGridParent = new GameObject("Grids");
    }

    private void UIOFF()
    {
        obFurniturePanel.SetActive(false);
        obMonsterPanel.SetActive(false);
    }

    private void ResetDropDowns()
    {
        ddTopWall = GameObject.Find("DdTopWall").GetComponent<Dropdown>();
        ddRightWall = GameObject.Find("DdRightWall").GetComponent<Dropdown>();
        ddBottomWall = GameObject.Find("DdBottomWall").GetComponent<Dropdown>();
        ddLeftWall = GameObject.Find("DdLeftWall").GetComponent<Dropdown>();
        ddMonType = GameObject.Find("DropdownMonsterType").GetComponent<Dropdown>();
        ddFurnType = GameObject.Find("DropdownFurnitureType").GetComponent<Dropdown>();
        ddNewFloor = GameObject.Find("DropdownNewFloor").GetComponent<Dropdown>();


        int monTypeMax = System.Enum.GetValues(typeof(MONSTERTYPE)).Length;
        string[] _typename = new string[monTypeMax];

        for (int i = 1; i < monTypeMax; i++)
        {
            MONSTERTYPE _type = (MONSTERTYPE)i;
            Dropdown.OptionData newData = new Dropdown.OptionData();

            _typename[i] = _type.ToString();
            newData.text = _typename[i];
            ddMonType.options.Add(newData);
        }
    }

    #endregion

    #region [Dataload]

    public void EditingStageInfo()
    {
        Text _savetext = GameObject.Find("TextSaveNewData").GetComponent<Text>();

        _savetext.text = string.Format("작업 내용을 {0}층으로 저장", ddNewFloor.value + 1);
        textNowfloor.text = string.Format("현재 층수 : {0}", ddNewFloor.value + 1);
        textFloorPattern.text = string.Format("저장된 맵 패턴 : {0}", DataIndex[ddNewFloor.value]);
    }
    
    public void LoadData()
    {
        DeleteALLMap();

        string _floor = inputFDataFileName.text.Substring(0, 1);

        textFloorPattern.text = string.Format("저장된 맵 패턴 : {0}", DataIndex[int.Parse(_floor) - 1]);
        textNowfloor.text = string.Format("현재 층수 : {0}", _floor);

        string filename = string.Format(@"{0}/Stage/{1}.map", Application.streamingAssetsPath, inputFDataFileName.text);
        editingfilename = inputFDataFileName.text;

        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = File.Open(filename, FileMode.Open);

        if (fs != null && fs.Length > 0)
        {
            mapDataArr = (TileData[,])bf.Deserialize(fs);

            fs.Close();
        }

        isLoadingData = true;
        GetDataParse();

        foreach (TileInfo _info in mapBoardArr)
        {
            if(_info != null)
            {
                Vector2 pos = _info.position;
                int x = (int)pos.x;
                int y = (int)pos.y;

                selectTilesList.Clear();

                selectTilesList.Add(pos);

                CreateMap();

                foreach (KeyValuePair<Vector2, MONSTERTYPE> _dicionary in _info.monSpawnInfoDic)
                {
                    Vector2 _pos = _dicionary.Key;

                    GameObject _monster = Instantiate(obMonPosCircle, new Vector2(_pos.x - 0.5f, _pos.y - 0.5f), Quaternion.identity);
                    _monster.name = string.Format("{0} // {1} MonPos", _pos.x, _pos.y);
                    _monster.transform.parent = mapBoardArr[x + 25, y + 25].obTile.transform;
                }

                foreach (KeyValuePair<Vector2, FurnitureInfo> _dicionary in _info.FurnitureInfoDic)
                {
                    Vector2 _pos = _dicionary.Key;

                    string _name = _dicionary.Value.name;

                    int _rotate = (int)_dicionary.Value.dir * 90;

                    obfurniture = Resources.Load("MapTools/Furniture/" + _name) as GameObject;

                    GameObject _furniture = Instantiate(obfurniture, new Vector2(_pos.x - 0.5f, _pos.y - 0.5f), Quaternion.Euler(new Vector3(-90 + _rotate, 90, -90)));
                    _furniture.transform.parent = mapBoardArr[x + 25, y + 25].obTile.transform;
                    _furniture.name = string.Format("{0} // {1} {2}", _pos.x, _pos.y, _name);
                }
            }
        }

        isEditData = true;
        isLoadingData = false;
    }

    private void GetDataParse()
    {
        foreach (TileData _data in mapDataArr)
        {
            if (_data != null)
            {
                TileInfo _info = new TileInfo();

                string[] xyPos = _data.pos.Split('/');
                int x = int.Parse(xyPos[0]);
                int y = int.Parse(xyPos[1]);

                Debug.Log(x + " // " + y);

                _info.position = new Vector2(x, y);
                _info.roomIndex = _data.roomIndex;
                _info.roomType = _data.roomType;
                _info.doorArr = _data.doorArr;

                foreach (KeyValuePair<string, MONSTERTYPE> _dicionary in _data.monSpawnInfoDic)
                {
                    string[] _xyPos = _dicionary.Key.Split('/');

                    float _x = float.Parse(_xyPos[0]);
                    float _y = float.Parse(_xyPos[1]);

                    MONSTERTYPE _type = _dicionary.Value;

                    _info.monSpawnInfoDic.Add(new Vector2(_x, _y), _type);
                }

                foreach (KeyValuePair<string, FurnitureData> _dicionary in _data.FurnitureInfoDic)
                {
                    FurnitureInfo _finfo = new FurnitureInfo();

                    string[] _xyPos = _dicionary.Key.Split('/');

                    float _x = float.Parse(_xyPos[0]);
                    float _y = float.Parse(_xyPos[1]);

                    FurnitureData _fdata = _dicionary.Value;

                    _finfo.name = _fdata.name;
                    _finfo.pos = new Vector2(_x, _y);
                    _finfo.dir = _fdata.dir;

                    _info.FurnitureInfoDic.Add(_finfo.pos, _finfo);
                }

                mapBoardArr[BoardPosParse(x), BoardPosParse(y)] = _info;
            }
        }
    }

    private void ReSetDataIndex()
    {
        for (int i = 1; i < 4; i++)
        {
            string filename = string.Format(@"{0}/Stage/{1}_DataIndex.idx", Application.streamingAssetsPath, i);
            int _index = 0;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            bf.Serialize(fs, _index);
            fs.Close();
        }
    }

    public void LoadDataIndex() 
    {
        for (int i = 0; i < 3; i++)
        {
            string filename = string.Format(@"{0}/Stage/{1}_DataIndex.idx", Application.streamingAssetsPath, i + 1);

            BinaryFormatter bf = new BinaryFormatter();

            FileStream fs = File.Open(filename, FileMode.Open);

            if (fs != null && fs.Length > 0)
            {
                DataIndex[i] = (int)bf.Deserialize(fs);
                //Debug.Log(DataIndex[i]);
                fs.Close();
            }
        }

        textFloorPattern.text = string.Format("저장된 맵 패턴 : {0}", DataIndex[ddNewFloor.value]);
    }    

    public void PasteData()
    {
        if (isEditData)
        {
            SetDataParse();

            string filename = string.Format(@"{0}/Stage/{1}.map", Application.streamingAssetsPath, editingfilename);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            bf.Serialize(fs, mapDataArr);
            fs.Close();
        }
        else
        {
            ErrorMessage("새로운 파일을 작성 중입니다.");
        }
    }

    public void OpenNewFile()
    {
        textNowfloor.text = string.Format("현재 층수 : {0}", ddNewFloor.value + 1);

        isEditData = false;

        for (int x = -25; x < 25; x++)
        {
            for (int y = -25; y < 25; y++)
            {
                Vector2 _pos = new Vector2(x, y);

                selectTilesList.Add(_pos);
            }
        }

        DeleteMap();
        mapBoardArr = new TileInfo[51, 51];
    }

    public void SaveData()
    {
        SetDataParse();

        string filename = string.Format(@"{0}/Stage/{1}F_{2}.map", Application.streamingAssetsPath, ddNewFloor.value + 1, DataIndex[ddNewFloor.value] + 1);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
        bf.Serialize(fs, mapDataArr);
        fs.Close();

        SaveDataIndex();
    }

    private void SaveDataIndex()
    {
        DataIndex[ddNewFloor.value]++;
        //Debug.Log(DataIndex[ddNewFloor.value]);
        string filename = string.Format(@"{0}/Stage/{1}_DataIndex.idx", Application.streamingAssetsPath, ddNewFloor.value + 1);
        int _index = DataIndex[ddNewFloor.value];
        //int _index = 0;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
        bf.Serialize(fs, _index);
        fs.Close();

        textFloorPattern.text = string.Format("저장된 맵 패턴 : {0}", DataIndex[ddNewFloor.value]);
        editingfilename = string.Format("{0}F_{1}", ddNewFloor.value + 1, DataIndex[ddNewFloor.value]);
        inputFDataFileName.text = editingfilename;
        isEditData = true;
    }

    private void SetDataParse()
    {
        mapDataArr = new TileData[51, 51];

        foreach (TileInfo _info in mapBoardArr)
        {
            if (_info != null)
            {
                TileData _data = new TileData();

                int x = (int)_info.position.x;
                int y = (int)_info.position.y;

                Debug.Log(x +" // "+ y);

                _data.pos = string.Format("{0}/{1}", x, y);
                _data.roomIndex = _info.roomIndex;
                _data.roomType = _info.roomType;
                _data.doorArr = _info.doorArr;

                foreach (KeyValuePair<Vector2, MONSTERTYPE> _dicionary in _info.monSpawnInfoDic)
                {
                    float _x = _dicionary.Key.x;
                    float _y = _dicionary.Key.y;
                    MONSTERTYPE _type = _dicionary.Value;

                    _data.monSpawnInfoDic.Add(string.Format("{0}/{1}", _x, _y), _type);
                }

                foreach (KeyValuePair<Vector2, FurnitureInfo> _dicionary in _info.FurnitureInfoDic)
                {
                    FurnitureData _fdata = new FurnitureData();

                    float _x = _dicionary.Key.x;
                    float _y = _dicionary.Key.y;

                    FurnitureInfo _finfo = _dicionary.Value;

                    _fdata.name = _finfo.name;
                    _fdata.pos = string.Format("{0}/{1}", _finfo.pos.x, _finfo.pos.y);
                    _fdata.dir = _finfo.dir;

                    _data.FurnitureInfoDic.Add(string.Format("{0}/{1}", _x, _y), _fdata);
                }

                mapDataArr[BoardPosParse(x), BoardPosParse(y)] = _data;
            }
        }
    }

    private void LoadFurnitureCSVData()
    {
        StreamReader streader = new StreamReader(Application.dataPath + "/StreamingAssets/CSV/FurnitureData.csv");
        
        while (!streader.EndOfStream)
        {
            string line = streader.ReadLine();

            string[] data = line.Split(',');

            furnitureDataList.Add(data);
        }

        for (int i = 1; i < furnitureDataList.Count; i++)
        {
            Dropdown.OptionData newData = new Dropdown.OptionData();

            newData.text = furnitureDataList[i][(int)FURNITUREDATA.NAME];
            ddFurnType.options.Add(newData);
        }
    }

    #endregion

    #region [ErrorMessage]

    private void ErrorMessage(string _errorText)
    {
        Text errorText = GameObject.Find("ErrorInfo").GetComponent<Text>();

        if(errorcoroutine != null)
        {
            StopCoroutine(errorcoroutine);
        }

        errorcoroutine = StartCoroutine(Error(errorText, _errorText));
    }

    static IEnumerator Error(Text _text, string _errorText)
    {
        _text.text = _errorText;

        yield return new WaitForSeconds(3.0f);

        _text.text = "";
    }

    #endregion
}