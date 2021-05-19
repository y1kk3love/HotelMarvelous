using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public MapManager mapmanager;

    public ROOMTYPE roomType;
    private ROOMDIR dir;

    private List<Vector2> floorPosArr = new List<Vector2>();

    private GameObject floorPrefab;     //초기화 필요

    private byte centerOnBoard;         //배열의 중심인 (25,25)를 (0,0)으로 놓고 사용하기위한 변수
    private byte curX, curY;


    void Start()
    {
        
        mapmanager = GameObject.Find("MapManager").GetComponent<MapManager>();
        centerOnBoard = (byte)(mapmanager.RoomBoard.Length / 2);

        floorPrefab = mapmanager.floorPrefab;

        curX = (byte)transform.position.x;
        curY = (byte)transform.position.y;

        dir = (ROOMDIR)Random.Range(0, System.Enum.GetValues(typeof(ROOMDIR)).Length);

        SpawnRoomFloor();
    }

    private void SpawnRoomFloor()
    {
        byte roomsize = Randomsize();                       //생성해야할 방의 타입을 확인한 후 그에 따라서 랜덤값 반환;
        byte dirCount = 0;                                  //4가되면 4방향 모두 막혔다는 이야기

        mapmanager.curRoomCount++;                          //타일끼리 비교하기 위한 방의 인덱스값 맵 생성 성공시 ++ (!!!!!!!!!!!!나중에 뒤로 이동시켜서 작성해줘야함!!!!!!!!!!!!!!!)

        for(int i = 0; i < roomsize; i++)
        {
            if (i == 0)                                      //첫 타일은 방의 좌표와 같은곳에 생성
            {
                GameObject _floor = Instantiate(floorPrefab, new Vector2(curX, curY), Quaternion.identity);
                _floor.transform.parent = transform;

                SetInfoToBoard(new Vector2(curX, curY), _floor, mapmanager.curRoomCount);
            }
            else
            {
                if(dirCount > 3)
                {
                    if(roomType == ROOMTYPE.HALLWAY)
                    {
                        mapmanager.RoomBoard[curX, curY] = null;
                        Destroy(gameObject);
                    }

                    roomsize++;
                    break;
                }

                switch (dir)
                {
                    case ROOMDIR.TOP:
                        if (mapmanager.RoomBoard[curX, curY + 1] == null)
                        {
                            GameObject _floor = Instantiate(floorPrefab, new Vector2(curX, curY + 1), Quaternion.identity);
                            _floor.transform.parent = transform;

                            SetInfoToBoard(new Vector2(curX, curY + 1), _floor, mapmanager.curRoomCount);

                            curY++;
                            dir = 0;
                        }
                        else
                        {
                            dir++;
                            break;
                        }
                        break;
                    case ROOMDIR.RIGHT:
                        if (mapmanager.RoomBoard[curX + 1, curY] == null)
                        {
                            GameObject _floor = Instantiate(floorPrefab, new Vector2(curX + 1, curY), Quaternion.identity);
                            _floor.transform.parent = transform;

                            SetInfoToBoard(new Vector2(curX + 1, curY), _floor, mapmanager.curRoomCount);

                            curX++;
                            dir = 0;
                        }
                        else
                        {
                            dir++;
                            break;
                        }
                        break;
                    case ROOMDIR.BOTTOM:
                        if (mapmanager.RoomBoard[curX, curY - 1] == null)
                        {
                            GameObject _floor = Instantiate(floorPrefab, new Vector2(curX, curY - 1), Quaternion.identity);
                            _floor.transform.parent = transform;

                            SetInfoToBoard(new Vector2(curX, curY - 1), _floor, mapmanager.curRoomCount);

                            curY--;
                            dir = 0;
                        }
                        else
                        {
                            dir++;
                            break;
                        }
                        break;
                    case ROOMDIR.LEFT:
                        if (mapmanager.RoomBoard[curX - 1, curY] == null)
                        {
                            GameObject _floor = Instantiate(floorPrefab, new Vector2(curX - 1, curY), Quaternion.identity);
                            _floor.transform.parent = transform;

                            SetInfoToBoard(new Vector2(curX - 1, curY), _floor, mapmanager.curRoomCount);

                            curX--;
                            dir = 0;
                        }
                        else
                        {
                            dir++;
                            break;
                        }
                        break;
                }
            }
        }
    }

    private void SpawnFloorWall()
    {
        ROOMDIR walldir = 0;

        foreach(Vector2 _pos in floorPosArr)
        {

        }
    }

    private void SetInfoToBoard(Vector2 _pos, GameObject _roomobj, byte _index)
    {
        FloorInfo floorinfo = new FloorInfo();

        floorinfo.floorPos = _pos;
        floorinfo.floorObject = _roomobj;
        floorinfo.roomIndex = _index;

        mapmanager.RoomBoard[PosParse(_pos.x), PosParse(_pos.y)] = floorinfo;

        floorPosArr.Add(_pos);
    }

    private byte PosParse(float _pos)
    {
        //배열에 값을 넣기위해 입력값에 배열 중앙값을 더해서 반환
        return (byte)(centerOnBoard + _pos);
    }

    private byte Randomsize()
    {
        //방 탑입에 따라 생성해야할 방의 수를 지정
        byte _value = 0;

        switch (roomType)
        {
            case ROOMTYPE.HALLWAY:
                _value = 2;
                break;
            case ROOMTYPE.GUEST:
                _value = (byte)Random.Range(1, 6);
                break;
        }

        return _value;
    }
}
