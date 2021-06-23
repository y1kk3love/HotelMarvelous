using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolManager : MonoBehaviour
{
    private Camera camera = null;

    private GameObject obRoomPick;
    private GameObject obPickedRoom;
    private GameObject obFloor;

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
        camera = Camera.main.GetComponent<Camera>();

        obRoomPick = Resources.Load("MapTools/Prefab/RoomPick") as GameObject;
        obFloor = Resources.Load("MapTools/Prefab/Floor") as GameObject;
    }

    void Update()
    {
        CameraDragMove();
        CameraWheelZoom();

        FloorPick();

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
            dragBPCamPos = camera.transform.position;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            dragGridStartPos = null;
        }

        if (Input.GetKey(KeyCode.Mouse1) && dragGridStartPos != null)
        {
            Vector2 dir = (Vector2)dragGridStartPos - dragBPCurPos;

            camera.transform.position = dragBPCamPos + dir * (camera.orthographicSize  / (minCamZoom + maxCamZoom) / 2);
        }
    }

    //카메라 휠 줌인 아웃
    private void CameraWheelZoom()           
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * cameraWheelSpeed * (camera.orthographicSize / -15.0f);

        if (camera.orthographicSize <= minCamZoom && scroll < 0)
        {
            camera.orthographicSize = minCamZoom;
        }
        else if(camera.orthographicSize >= maxCamZoom && scroll > 0)
        {
            camera.orthographicSize = maxCamZoom;
        }
        else
        {
            camera.orthographicSize += scroll;
        }
    }

    #endregion

    #region [MapSelect]
    private void FloorPick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
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

                if (!isRoomSelect)
                {
                    isRoomSelect = true;

                    obPickedRoom = Instantiate(obRoomPick, new Vector2(curTileX, curTileY), Quaternion.identity);
                }
                else
                {
                    obPickedRoom.transform.position = new Vector2(curTileX, curTileY);
                }
            }
        }
    }

    #endregion

    #region [MapControl]

    private void MapControl()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateMap();
        }

        if (Input.GetKeyDown(KeyCode.D))
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

    public WALLSTATE[] doorArr = new WALLSTATE[4];
}
