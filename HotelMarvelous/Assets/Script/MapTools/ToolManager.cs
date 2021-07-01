using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToolManager : MonoBehaviour
{
    private Camera bpCamera;                                     //청사진 카메라

    private GameObject obRoomPick;                               //17x17 그리드 리소스
    private GameObject obPickedRoom;                             //생성된 17x17 그리드의 오브젝트
    private GameObject obFloor;                                  //타일 바닥 리소스
    private GameObject obBlockWall;                              //막힌 벽 리소스
    private GameObject obDoorWall;                               //문있는 벽 리소스
    private GameObject obDragBox;                                //드래그 박스 리소스
    private GameObject obCurDragBox = null;                      //현재 생성된 드래그 박스
    private GameObject obCurDragSelectBox = null;                //현재 생성된 드래스 선택 박스들을 넣은 빈오브젝트

    private Text textTilePos;                                    //선택된 타일의 좌표, 방인덱스등이 나오는 텍스트
    
    private InputField InputFRoomIndex;                            //방의 인덱스를 입력받는 인풋필드

    private Dropdown dbTopWall;                                  //타일 벽을 설정하기 위한 드롭다운들
    private Dropdown dbRightWall;
    private Dropdown dbBottomWall;
    private Dropdown dbLeftWall;

    private TileInfo[,] mapBoardArr = new TileInfo[51, 51];      //타일의 정보를 담은 클래스를 가진 배열
    private TileInfo curTile = null;                   //현재 선택된 타일의 정보

    private int curTileX, curTileY;                              //현재 선택한 타일의 실제 좌표 18/1사이즈

    private float cameraWheelSpeed = 20.0f;                      //카메라 줌 스피드
    private float minCamZoom = 5.0f;                             //카메라 줌 최소사이즈
    private float maxCamZoom = 450.0f;                           //카메라 줌 최대사이즈

    private Vector2? dragGridStartPos = null;                    //카메라 이동을 시작한 위치, 기준점
    private Vector2 dragBPCurPos;                                //현재 마우스의 위치
    private Vector2 dragBPCamPos;                                //카메라 이동을 시작했을때 카메라의 위치

    private Vector2 dragBoxStartPos;                             //드래그 박스를 위한 좌표
    private Vector2 dragBoxCurPos;

    private List<Vector2> selectTilesList = new List<Vector2>();

    private bool isGridMode = false;
    private bool isTileSelect = false;                           //타일이 선택되었는지 확인
    private bool isWallChanging = false;                         //UI를 통해 벽을 수정중인지 확인

    void Start()
    {
        bpCamera = Camera.main.GetComponent<Camera>();

        obRoomPick = Resources.Load("MapTools/Prefab/RoomPick") as GameObject;
        obFloor = Resources.Load("MapTools/Prefab/Floor") as GameObject;
        obBlockWall = Resources.Load("MapTools/Prefab/Wall") as GameObject;
        obDoorWall = Resources.Load("MapTools/Prefab/Door") as GameObject;
        obDragBox = Resources.Load("MapTools/Prefab/BoxSelect") as GameObject;

        dbTopWall = GameObject.Find("DdTopWall").GetComponent<Dropdown>();
        dbRightWall = GameObject.Find("DdRightWall").GetComponent<Dropdown>();
        dbBottomWall = GameObject.Find("DdBottomWall").GetComponent<Dropdown>();
        dbLeftWall = GameObject.Find("DdLeftWall").GetComponent<Dropdown>();

        textTilePos = GameObject.Find("TextTilePos").GetComponent<Text>();
        InputFRoomIndex = GameObject.Find("InputFieldRoomNum").GetComponent<InputField>();

        obPickedRoom = new GameObject("Grids");
    }

    void Update()
    {
        //카메라 이동과 줌
        CameraDragMove();
        CameraWheelZoom();

        //타일 선택과 인덱스 단축키
        FloorPick();
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

            bpCamera.transform.position = dragBPCamPos + dir * (bpCamera.orthographicSize  / (minCamZoom + maxCamZoom) / 2);
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

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector2 _Pos = hit.point;
                string _index;
                ROOMTYPE _type = ROOMTYPE.EMPTY;

                dragBoxStartPos = _Pos;
                curTileX = EditPosParse(_Pos.x);
                curTileY = EditPosParse(_Pos.y);

                isWallChanging = false;

                //Debug.Log(" 선택된 좌표 : " + _Pos);

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
                    //obPickedRoom = Instantiate(obRoomPick, new Vector2(curTileX, curTileY), Quaternion.identity);
                }
                else
                {
                    textTilePos.text = string.Format("Tile X : {0} // Y : {1}\nIndex {2} // Type {3}", (curTileX / 18), (curTileY / 18), _index, _type);
                    //obPickedRoom.transform.position = new Vector2(curTileX, curTileY);
                    curTile = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)];

                    if (mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)] != null)
                    {
                        InputFRoomIndex.text = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].roomIndex.ToString();

                        for (int i = 0; i < 4; i++)
                        {
                            switch (i)
                            {
                                case (int)DIRECTION.TOP:
                                    dbTopWall.value = (byte)curTile.doorArr[i];
                                    break;
                                case (int)DIRECTION.RIGHT:
                                    dbRightWall.value = (byte)curTile.doorArr[i];
                                    break;
                                case (int)DIRECTION.BOTTOM:
                                    dbBottomWall.value = (byte)curTile.doorArr[i];
                                    break;
                                case (int)DIRECTION.LEFT:
                                    dbLeftWall.value = (byte)curTile.doorArr[i];
                                    break;
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.Mouse0) && !isGridMode)
        {
            if(obCurDragBox == null)
            {
                obCurDragBox = Instantiate(obDragBox, dragBoxStartPos, Quaternion.identity);
            }

            dragBoxCurPos = hit.point;

            obCurDragBox.transform.position = ((dragBoxStartPos + dragBoxCurPos) / 2);
            obCurDragBox.transform.localScale = new Vector2(Mathf.Abs(dragBoxStartPos.x - dragBoxCurPos.x), Mathf.Abs(dragBoxStartPos.y - dragBoxCurPos.y));
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && !isGridMode)
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
            if (InputFRoomIndex.text == "")
            {
                InputFRoomIndex.text = "1";
            }
            else
            {
                InputFRoomIndex.text = (byte.Parse(InputFRoomIndex.text) + 1).ToString();
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (InputFRoomIndex.text == "")
            {
                InputFRoomIndex.text = "0";
            }
            else
            {
                if (byte.Parse(InputFRoomIndex.text) > 0)
                {
                    InputFRoomIndex.text = (byte.Parse(InputFRoomIndex.text) - 1).ToString();
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
                foreach (Vector2 _pos in selectTilesList)
                {
                    int _x = (int)_pos.x;
                    int _y = (int)_pos.y;

                    if (mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] != null)
                    {
                        TileInfo _info = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)];

                        _info.roomType = ROOMTYPE.EMPTY;
                        _info.obTile.transform.Find("Floor(Clone)").GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }

                textTilePos.text = string.Format("Tile X : {0} // Y : {1}\nIndex {2} // Type {3}", (curTileX / 18), (curTileY / 18), curTile.roomIndex, ROOMTYPE.EMPTY);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                foreach (Vector2 _pos in selectTilesList)
                {
                    int _x = (int)_pos.x;
                    int _y = (int)_pos.y;

                    if (mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] != null)
                    {
                        TileInfo _info = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)];

                        _info.roomType = ROOMTYPE.HALLWAY;
                        _info.obTile.transform.Find("Floor(Clone)").GetComponent<SpriteRenderer>().color = Color.red;
                    }
                }

                textTilePos.text = string.Format("Tile X : {0} // Y : {1}\nIndex {2} // Type {3}", (curTileX / 18), (curTileY / 18), curTile.roomIndex, ROOMTYPE.HALLWAY);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                foreach (Vector2 _pos in selectTilesList)
                {
                    int _x = (int)_pos.x;
                    int _y = (int)_pos.y;

                    if (mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] != null)
                    {
                        TileInfo _info = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)];

                        _info.roomType = ROOMTYPE.GUEST;
                        _info.obTile.transform.Find("Floor(Clone)").GetComponent<SpriteRenderer>().color = Color.yellow;
                    }
                }

                textTilePos.text = string.Format("Tile X : {0} // Y : {1}\nIndex {2} // Type {3}", (curTileX / 18), (curTileY / 18), curTile.roomIndex, ROOMTYPE.GUEST);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                foreach (Vector2 _pos in selectTilesList)
                {
                    int _x = (int)_pos.x;
                    int _y = (int)_pos.y;

                    if (mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] != null)
                    {
                        TileInfo _info = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)];

                        _info.roomType = ROOMTYPE.NPC;
                        _info.obTile.transform.Find("Floor(Clone)").GetComponent<SpriteRenderer>().color = Color.green;
                    }
                }

                textTilePos.text = string.Format("Tile X : {0} // Y : {1}\nIndex {2} // Type {3}", (curTileX / 18), (curTileY / 18), curTile.roomIndex, ROOMTYPE.NPC);
            }
        }
    }

    #endregion

    #region [MapControl]

    private void MapControl()
    {
        //타일 생성 단축키
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateMap();
        }

        //타일 삭제 단축키
        if (Input.GetKeyDown(KeyCode.E))
        {
            DeleteMap();
        }
    }

    public void CreateMap()
    {
        if (!isGridMode)
        {
            foreach (Vector2 _pos in selectTilesList)
            {
                int _x = (int)_pos.x;
                int _y = (int)_pos.y;

                if (mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] == null)
                {
                    GameObject emptytile = new GameObject(string.Format("Tile/{0},{1}", (_x / 18), (_y / 18)));
                    emptytile.transform.position = new Vector3(_x, _y, 9);

                    GameObject floor = Instantiate(obFloor, new Vector3(_x, _y, 9), Quaternion.identity);
                    floor.transform.parent = emptytile.transform;

                    mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] = new TileInfo();
                    mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].obTile = emptytile;

                    if (InputFRoomIndex.text == "")        //최초 실행일때 인덱스값 1 설정
                    {
                        InputFRoomIndex.text = "1";
                        mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].roomIndex = 1;
                    }
                    else
                    {
                        mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].roomIndex = byte.Parse(InputFRoomIndex.text);
                    }

                    textTilePos.text = string.Format("Tile X : {0} // Y : {1}\nIndex {2} // Type {3}", (_x / 18), (_y / 18), mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].roomIndex, ROOMTYPE.EMPTY);

                    curTile = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)];
                }
            }

            foreach (Vector2 _pos in selectTilesList)
            {
                BuildWall(mapBoardArr[BoardPosParse((int)_pos.x), BoardPosParse((int)_pos.y)]);       //생성된 타일 위에 벽 생성
            }
        }
    }

    public void DeleteMap()
    {
        if (!isGridMode)
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

                        dbTopWall.value = (byte)_tileinfo.doorArr[i];
                    }
                    else
                    {
                        dbTopWall.value = 1;
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

                        dbRightWall.value = (byte)_tileinfo.doorArr[i];
                    }
                    else
                    {
                        dbRightWall.value = 1;
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

                        dbBottomWall.value = (byte)_tileinfo.doorArr[i];
                    }
                    else
                    {
                        dbBottomWall.value = 1;
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

                        dbLeftWall.value = (byte)_tileinfo.doorArr[i];
                    }
                    else
                    {
                        dbLeftWall.value = 1;
                    }

                    break;
            }
        }
    }

    public void OnDropdownnEvent(int index)
    {
        if (curTile != null && isWallChanging)
        {
            curTile.doorArr[(byte)DIRECTION.TOP] = (WALLSTATE)dbTopWall.value;
            curTile.doorArr[(byte)DIRECTION.RIGHT] = (WALLSTATE)dbRightWall.value;
            curTile.doorArr[(byte)DIRECTION.BOTTOM] = (WALLSTATE)dbBottomWall.value;
            curTile.doorArr[(byte)DIRECTION.LEFT] = (WALLSTATE)dbLeftWall.value;

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

        if (!isGridMode)
        {
            isGridMode = true;
            _buttontext.text = "가구 배치중";

            for(int _x = -25; _x < 25; _x++)
            {
                for (int _y = -25; _y < 25; _y++)
                {
                    if(mapBoardArr[BoardPosParse(_x * 18), BoardPosParse(_y * 18)] != null)
                    {
                        GameObject _grid = Instantiate(obRoomPick, new Vector2(_x * 18, _y * 18), Quaternion.identity);
                        _grid.transform.parent = obPickedRoom.transform;
                    }
                }
            }
        }
        else
        {
            isGridMode = false;
            _buttontext.text = "방 생성중";

            Destroy(obPickedRoom);
            obPickedRoom = new GameObject("Grid");
        }
    }

    #endregion

    #region [Calculator]

    //청사진에서의 움직임을 위한 변환
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
}

class TileInfo
{
    public GameObject obTile;

    public byte roomIndex;

    public ROOMTYPE roomType = ROOMTYPE.EMPTY;

    public WALLSTATE[] doorArr = new WALLSTATE[4];
}
