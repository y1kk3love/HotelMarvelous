using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToolManager : MonoBehaviour
{
    private Camera bpCamera;

    private GameObject obRoomPick;
    private GameObject obPickedRoom;
    private GameObject obFloor;
    private GameObject obBlockWall;
    private GameObject obDoorWall;

    private Text textTilePos;
    private InputField textRoomIndex;

    private TileInfo[,] mapBoardArr = new TileInfo[51, 51];

    private int curTileX, curTileY;

    private float cameraWheelSpeed = 20.0f;
    private float minCamZoom = 5.0f;
    private float maxCamZoom = 450.0f;
    private float maxTileSize;

    private Vector2? dragGridStartPos = null;
    private Vector2 dragBPCurPos;
    private Vector2 dragBPCamPos;

    private bool isRoomSelect = false;

    void Start()
    {
        bpCamera = Camera.main.GetComponent<Camera>();

        obRoomPick = Resources.Load("MapTools/Prefab/RoomPick") as GameObject;
        obFloor = Resources.Load("MapTools/Prefab/Floor") as GameObject;
        obBlockWall = Resources.Load("MapTools/Prefab/Wall") as GameObject;
        obDoorWall = Resources.Load("MapTools/Prefab/Door") as GameObject;

        textTilePos = GameObject.Find("TextTilePos").GetComponent<Text>();
        textRoomIndex = GameObject.Find("InputFieldRoomNum").GetComponent<InputField>();
    }

    void Update()
    {
        CameraDragMove();
        CameraWheelZoom();

        FloorPick();
        IndexUpper();

        MapControl();
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = bpCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                Vector2 _Pos = hit.point;

                curTileX = EditPosParse(_Pos.x);
                curTileY = EditPosParse(_Pos.y);

                //Debug.Log(" 선택된 좌표 : " + _Pos);
                string _index;

                if (mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)] != null)
                {
                    _index = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].roomIndex.ToString();
                }
                else
                {
                    _index = "-1";
                }

                if (!isRoomSelect)
                {
                    isRoomSelect = true;

                    textTilePos.text = string.Format("Tile X : {0} //  Y : {1} // Index {2}", (curTileX / 18), (curTileY / 18), _index);
                    obPickedRoom = Instantiate(obRoomPick, new Vector2(curTileX, curTileY), Quaternion.identity);
                }
                else
                {
                    textTilePos.text = string.Format("Tile X : {0} //  Y : {1} // Index {2}", (curTileX / 18), (curTileY / 18), _index);
                    obPickedRoom.transform.position = new Vector2(curTileX, curTileY);

                    if (mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)] != null)
                    {
                        textRoomIndex.text = mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].roomIndex.ToString();
                    }
                }
            }
        }
    }

    private void IndexUpper()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (textRoomIndex.text == "")
            {
                textRoomIndex.text = "1";
            }
            else
            {
                textRoomIndex.text = (byte.Parse(textRoomIndex.text) + 1).ToString();
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (textRoomIndex.text == "")
            {
                textRoomIndex.text = "0";
            }
            else
            {
                if (byte.Parse(textRoomIndex.text) > 0)
                {
                    textRoomIndex.text = (byte.Parse(textRoomIndex.text) - 1).ToString();
                }
            }
        }
    }

    #endregion

    #region [MapControl]

    private void MapControl()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateMap();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            DeleteMap();
        }
    }

    public void CreateMap()
    {
        if (mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)] == null)
        {
            GameObject emptytile = new GameObject(string.Format("Tile/{0},{1}", (BoardPosParse(curTileX) - 25), (BoardPosParse(curTileY)) - 25));
            emptytile.transform.position = new Vector3(curTileX, curTileY, 9);

            GameObject floor = Instantiate(obFloor, new Vector3(curTileX, curTileY, 9), Quaternion.identity);
            floor.transform.parent = emptytile.transform;

            mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)] = new TileInfo();
            mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].obTile = emptytile;

            if(textRoomIndex.text == "")
            {
                textRoomIndex.text = "1";
                mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].roomIndex = 1;
            }
            else
            {
                mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].roomIndex = byte.Parse(textRoomIndex.text);
            }

            BuildWall(mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)]);
            textTilePos.text = string.Format("Tile X : {0} //  Y : {1} // Index {2}", (curTileX / 18), (curTileY / 18), mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].roomIndex);
        }
    }

    public void DeleteMap()
    {
        if (mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)] != null)
        {
            Destroy(mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)].obTile);

            mapBoardArr[BoardPosParse(curTileX), BoardPosParse(curTileY)] = null;
        }
    }

    private void BuildWall(TileInfo _tileinfo)
    {
        if (_tileinfo.obTile.transform.Find("Walls") != null)
        {
            Destroy(_tileinfo.obTile.transform.Find("Walls").gameObject);
        }

        GameObject emptywall;

        emptywall = new GameObject("Walls");
        emptywall.transform.parent = _tileinfo.obTile.transform;

        for (int i = 0; i < 4; i++)
        {
            GameObject _wall = null;

            Vector2 _pos = _tileinfo.obTile.transform.position;

            switch (_tileinfo.doorArr[i])
            {
                case WALLSTATE.BLOCK:
                    _wall = obBlockWall;
                    break;
                case WALLSTATE.EMPTY:
                    return;
                case WALLSTATE.DOOR:
                    _wall = obDoorWall;
                    break;
            }

            switch (i)
            {
                case (int)DIRECTION.TOP:
                    GameObject topWall = Instantiate(_wall, new Vector2(_pos.x, _pos.y + 8.5f), Quaternion.Euler(0, 0, 90));
                    topWall.transform.parent = emptywall.transform;
                    break;
                case (int)DIRECTION.RIGHT:
                    GameObject rightWall = Instantiate(_wall, new Vector2(_pos.x + 8.5f, _pos.y), Quaternion.Euler(0, 0, 0));
                    rightWall.transform.parent = emptywall.transform;
                    break;
                case (int)DIRECTION.BOTTOM:
                    GameObject bottomWall = Instantiate(_wall, new Vector2(_pos.x, _pos.y - 8.5f), Quaternion.Euler(0, 0, -90));
                    bottomWall.transform.parent = emptywall.transform;
                    break;
                case (int)DIRECTION.LEFT:
                    GameObject leftWall = Instantiate(_wall, new Vector2(_pos.x - 8.5f, _pos.y), Quaternion.Euler(0, 0, -180));
                    leftWall.transform.parent = emptywall.transform;
                    break;
            }
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

    private byte BoardPosParse(int _pos)
    {
        return (byte)(_pos / 9 + 25);
    }

    #endregion
}

class TileInfo
{
    public GameObject obTile;

    public byte roomIndex;

    public WALLSTATE[] doorArr = new WALLSTATE[4];
}
