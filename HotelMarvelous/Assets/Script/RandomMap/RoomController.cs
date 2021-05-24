using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public MapManager mapmanager;

    public ROOMTYPE roomType;
    public DIRECTION? enterDir = null;
    private DIRECTION roomDir;                //4방향이 모두 막혔는지 확인하기 위한 카운터

    private List<Vector2> floorPosList = new List<Vector2>();        //생성된 바닥의 좌표를 가지고 있는 리스트
    private List<Vector2> nextFloorPosList = new List<Vector2>();    //다음으로 생성될 방의 좌표

    private GameObject floorPrefab;           //초기화 필요
    private GameObject wallPrefab;
    private GameObject doorPrefab;

    private byte centerOnBoard;               //배열의 중심인 (25,25)를 (0,0)으로 놓고 사용하기위한 변수
    private sbyte curX;
    private sbyte curY;
    private byte doorCount = 0;


    void Start()
    {
        mapmanager = GameObject.Find("MapManager").GetComponent<MapManager>();
        centerOnBoard = (byte)(Mathf.Sqrt(mapmanager.RoomBoard.Length) / 2);

        floorPrefab = mapmanager.floorPrefab;
        wallPrefab = mapmanager.wallPrefab;
        doorPrefab = mapmanager.doorPrefab;

        curX = (sbyte)transform.position.x;
        curY = (sbyte)transform.position.y;

        roomDir = (DIRECTION)Random.Range(0, System.Enum.GetValues(typeof(DIRECTION)).Length);

        SpawnRoomFloor();
        SpawnFloorWall();
    }

    private void SpawnRoomFloor()
    {
        if (mapmanager.curRoomCount < mapmanager.maxRoomCount && mapmanager.RoomBoard[PosParse(curX), PosParse(curY)] == null)
        {
            byte roomsize = Randomsize();                       //생성해야할 방의 타입을 확인한 후 그에 따라서 랜덤값 반환;
            byte dirblockcheck = 0;                             //4방향 모두 막혀있을 경우 반복문 탈출

            for (int i = 0; i < roomsize; i++)
            {
                if(dirblockcheck > 4)
                {
                    i++;
                    return;
                }

                if (i == 0)                                      //첫 타일은 방의 좌표와 같은곳에 생성
                {
                    GameObject _floor = Instantiate(floorPrefab, new Vector2(curX, curY), Quaternion.identity);
                    _floor.transform.parent = transform;

                    SetInfoToBoard(_floor, mapmanager.curRoomCount);
                }
                else
                {
                    if (roomDir > DIRECTION.LEFT)
                    {
                        roomDir = DIRECTION.TOP;
                    }

                    switch (roomDir)
                    {
                        case DIRECTION.TOP:
                            if (mapmanager.RoomBoard[PosParse(curX), PosParse(curY + 1)] == null)       //보드의 공간이 비어있다면
                            {
                                GameObject _floor = Instantiate(floorPrefab, new Vector2(curX, curY + 1), Quaternion.identity);         //바닥을 생성하고 방의 자식으로 넣는다.
                                _floor.transform.parent = transform;

                                SetInfoToBoard(_floor, mapmanager.curRoomCount);                           //보드좌표에 생성된 바닥과 생성된 순서(방끼리 구분하기 위해)를 넣는다.

                                curY++;                      //생성된 경우 좌표값을 변경해줍니다.
                                dirblockcheck = 0;           //바닥 생성에 성공했을 경우 0으로 초기화
                                roomDir = (DIRECTION)Random.Range(0, System.Enum.GetValues(typeof(DIRECTION)).Length);                  //방향 카운터를 랜덤 초기화
                            }
                            else
                            {
                                i--;
                                dirblockcheck++;
                                roomDir++;          //바닥 생성이 불가능할 경우 방향 카운터++
                            }
                            break;
                        case DIRECTION.RIGHT:
                            if (mapmanager.RoomBoard[PosParse(curX + 1), PosParse(curY)] == null)
                            {
                                GameObject _floor = Instantiate(floorPrefab, new Vector2(curX + 1, curY), Quaternion.identity);
                                _floor.transform.parent = transform;

                                SetInfoToBoard(_floor, mapmanager.curRoomCount);

                                curX++;
                                dirblockcheck = 0;
                                roomDir = (DIRECTION)Random.Range(0, System.Enum.GetValues(typeof(DIRECTION)).Length);
                            }
                            else
                            {
                                i--;
                                dirblockcheck++;
                                roomDir++;
                            }
                            break;
                        case DIRECTION.BOTTOM:
                            if (mapmanager.RoomBoard[PosParse(curX), PosParse(curY - 1)] == null)
                            {
                                GameObject _floor = Instantiate(floorPrefab, new Vector2(curX, curY - 1), Quaternion.identity);
                                _floor.transform.parent = transform;

                                SetInfoToBoard(_floor, mapmanager.curRoomCount);

                                curY--;
                                dirblockcheck = 0;
                                roomDir = (DIRECTION)Random.Range(0, System.Enum.GetValues(typeof(DIRECTION)).Length);
                            }
                            else
                            {
                                i--;
                                dirblockcheck++;
                                roomDir++;
                            }
                            break;
                        case DIRECTION.LEFT:
                            if (mapmanager.RoomBoard[PosParse(curX - 1), PosParse(curY)] == null)
                            {
                                GameObject _floor = Instantiate(floorPrefab, new Vector2(curX - 1, curY), Quaternion.identity);
                                _floor.transform.parent = transform;

                                SetInfoToBoard(_floor, mapmanager.curRoomCount);

                                curX--;
                                dirblockcheck = 0;
                                roomDir = (DIRECTION)Random.Range(0, System.Enum.GetValues(typeof(DIRECTION)).Length);
                            }
                            else
                            {
                                i--;
                                dirblockcheck++;
                                roomDir++;
                            }
                            break;
                    }
                }
            }
            mapmanager.curRoomCount++;                          //타일끼리 비교하기 위한 방의 인덱스값 맵 생성 성공시
        }
    }

    private void SpawnFloorWall()
    {
        byte randomdir = RandomWallDir();                               //만약 첫번째 방이 아닐 경우 이전방의 문 방향 값을 받아온다
        byte percant = 3;                                               //문이 생길 확률 (1/percant)

        foreach (Vector2 _pos in floorPosList)                           //벽을 만들 바닥의 배열
        {
            WALLSTATE[] wallcheckarr = new WALLSTATE[4];                    //그 바닥의 4방향 벽상태

            byte spawndir = 0;

            #region [CheckWallDir]

            foreach (Vector2 _checkpos in floorPosList)                  //같은 방의 바닥끼리 벽을 비우기 위해 비교할 바닥의 배열
            {
                for (byte i = randomdir; i < 4 + randomdir; i++)         //문이 달린 벽을 랜덤하게 생성해 주기 위한 순서
                {
                    byte _dir = 0;

                    if(i > 3)
                    {
                        _dir = (byte)(i - 4);
                    }
                    else
                    {
                        _dir = i;
                    }

                    switch (_dir)
                    {
                        case (byte)DIRECTION.TOP:

                            if(wallcheckarr[(byte)DIRECTION.TOP] == WALLSTATE.EMPTY)
                            {
                                break;
                            }

                            FloorInfo topInfo = mapmanager.RoomBoard[PosParse(_pos.x), PosParse(_pos.y + 1)];

                            if (new Vector2(_pos.x, _pos.y + 1) == _checkpos)           //확인한 칸에 이미 같은 방의 바닥이 있으면 벽을 비운다.
                            {
                                if(wallcheckarr[(byte)DIRECTION.TOP] == WALLSTATE.DOOR)
                                {
                                    doorCount--;
                                }

                                wallcheckarr[(byte)DIRECTION.TOP] = WALLSTATE.EMPTY;
                            }
                            else if (topInfo == null && doorCount < 4)       //보드에 값이 비어있고 문이 4개를 넘지 않으면 일정 확률로 문만들고 방 생성
                            {
                                if (mapmanager.curRoomCount < mapmanager.maxRoomCount)
                                {
                                    if (doorCount == 0)
                                    {
                                        wallcheckarr[(byte)DIRECTION.TOP] = WALLSTATE.DOOR;

                                        doorCount++;
                                    }
                                    else if (Random.Range(1, percant) == 1)
                                    {
                                        wallcheckarr[(byte)DIRECTION.TOP] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector2(_pos.x, _pos.y + 1));

                                        doorCount++;
                                    }
                                }
                                else
                                {
                                    topInfo.wallObj[(byte)DIRECTION.BOTTOM] = wallPrefab;
                                    Destroy(gameObject);
                                }
                            }
                            break;
                        case (byte)DIRECTION.RIGHT:

                            if (wallcheckarr[(byte)DIRECTION.RIGHT] == WALLSTATE.EMPTY)
                            {
                                break;
                            }

                            FloorInfo rightInfo = mapmanager.RoomBoard[PosParse(_pos.x + 1), PosParse(_pos.y)];

                            if (new Vector2(_pos.x + 1, _pos.y) == _checkpos)
                            {
                                if (wallcheckarr[(byte)DIRECTION.RIGHT] == WALLSTATE.DOOR)
                                {
                                    doorCount--;
                                }

                                wallcheckarr[(byte)DIRECTION.RIGHT] = WALLSTATE.EMPTY;
                            }
                            else if (rightInfo == null && doorCount < 4)
                            {
                                if (mapmanager.curRoomCount < mapmanager.maxRoomCount)
                                {
                                    if (doorCount == 0)
                                    {
                                        wallcheckarr[(byte)DIRECTION.RIGHT] = WALLSTATE.DOOR;

                                        doorCount++;
                                    }
                                    else if (Random.Range(1, percant) == 1)
                                    {
                                        wallcheckarr[(byte)DIRECTION.RIGHT] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector2(_pos.x + 1, _pos.y));

                                        doorCount++;
                                    }
                                }
                                else
                                {
                                    rightInfo.wallObj[(byte)DIRECTION.LEFT] = wallPrefab;
                                    Destroy(gameObject);
                                }
                            }
                            break;
                        case (byte)DIRECTION.BOTTOM:

                            if (wallcheckarr[(byte)DIRECTION.BOTTOM] == WALLSTATE.EMPTY)
                            {
                                break;
                            }

                            FloorInfo borromInfo = mapmanager.RoomBoard[PosParse(_pos.x), PosParse(_pos.y - 1)];

                            if (new Vector2(_pos.x, _pos.y - 1) == _checkpos)
                            {
                                if (wallcheckarr[(byte)DIRECTION.BOTTOM] == WALLSTATE.DOOR)
                                {
                                    doorCount--;
                                }

                                wallcheckarr[(byte)DIRECTION.BOTTOM] = WALLSTATE.EMPTY;
                            }
                            else if (borromInfo == null && doorCount < 4)
                            {
                                if (mapmanager.curRoomCount < mapmanager.maxRoomCount)
                                {
                                    if (doorCount == 0)
                                    {
                                        wallcheckarr[(byte)DIRECTION.BOTTOM] = WALLSTATE.DOOR;

                                        doorCount++;
                                    }
                                    else if (Random.Range(1, percant) == 1)
                                    {
                                        wallcheckarr[(byte)DIRECTION.BOTTOM] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector2(_pos.x, _pos.y - 1));

                                        doorCount++;
                                    }
                                }
                                else
                                {
                                    borromInfo.wallObj[(byte)DIRECTION.TOP] = wallPrefab;
                                    Destroy(gameObject);
                                }
                            }
                            break;
                        case (byte)DIRECTION.LEFT:

                            if (wallcheckarr[(byte)DIRECTION.LEFT] == WALLSTATE.EMPTY)
                            {
                                break;
                            }

                            FloorInfo leftInfo = mapmanager.RoomBoard[PosParse(_pos.x - 1), PosParse(_pos.y)];

                            if (new Vector2(_pos.x - 1, _pos.y) == _checkpos)
                            {
                                if (wallcheckarr[(byte)DIRECTION.LEFT] == WALLSTATE.DOOR)
                                {
                                    doorCount--;
                                }

                                wallcheckarr[(byte)DIRECTION.LEFT] = WALLSTATE.EMPTY;
                            }
                            else if (leftInfo == null && doorCount < 4)
                            {
                                if (mapmanager.curRoomCount < mapmanager.maxRoomCount)
                                {
                                    if (doorCount == 0)
                                    {
                                        wallcheckarr[(byte)DIRECTION.LEFT] = WALLSTATE.DOOR;

                                        doorCount++;
                                    }
                                    else if (Random.Range(1, percant) == 1)
                                    {
                                        wallcheckarr[(byte)DIRECTION.LEFT] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector2(_pos.x - 1, _pos.y));

                                        doorCount++;
                                    }
                                }
                                else
                                {
                                    leftInfo.wallObj[(byte)DIRECTION.RIGHT] = wallPrefab;
                                    Destroy(gameObject);
                                }
                            }
                            break;
                    }
                }
            }

            #endregion

            #region [SpawnWall]

            foreach (WALLSTATE wallstate in wallcheckarr)
            {
                sbyte _x = (sbyte)_pos.x;
                sbyte _y = (sbyte)_pos.y;

                int wallrotaion = 0;
                Vector3 wallpos = new Vector3(0, 0, 0);
                GameObject floor = mapmanager.RoomBoard[PosParse(_x), PosParse(_y)].floorObject;

                switch (spawndir)
                {
                    case (byte)DIRECTION.TOP:
                        wallpos = new Vector3(_x, _y + 0.45f, -0.05f);
                        wallrotaion = 90;
                        break;
                    case (byte)DIRECTION.RIGHT:
                        wallpos = new Vector3(_x + 0.45f, _y, -0.05f);
                        wallrotaion = 0;
                        break;
                    case (byte)DIRECTION.BOTTOM:
                        wallpos = new Vector3(_x, _y - 0.45f, -0.05f);
                        wallrotaion = -90;
                        break;
                    case (byte)DIRECTION.LEFT:
                        wallpos = new Vector3(_x - 0.45f, _y, -0.05f);
                        wallrotaion = -180;
                        break;
                }

                if (wallstate == WALLSTATE.BLOCK)
                {
                    Instantiate(wallPrefab, wallpos, Quaternion.Euler(0, 0, wallrotaion)).transform.parent = floor.transform;
                }
                else if(wallstate == WALLSTATE.DOOR)
                {
                    Instantiate(doorPrefab, wallpos, Quaternion.Euler(0, 0, wallrotaion)).transform.parent = floor.transform;
                }

                spawndir++;
            }

            #endregion
        }

        Debug.Log(doorCount);
    }

    private void SetInfoToBoard(GameObject _roomobj, byte _index)
    {
        FloorInfo floorinfo = new FloorInfo();
        Vector2 _pos = new Vector2(_roomobj.transform.position.x, _roomobj.transform.position.y);

        floorinfo.floorObject = _roomobj;
        floorinfo.roomIndex = _index;
        floorinfo.roomType = roomType;

        mapmanager.RoomBoard[PosParse(_pos.x), PosParse(_pos.y)] = floorinfo;
        
        floorPosList.Add(_pos);
    }

    private byte PosParse(float _pos)
    {
        //배열에 값을 넣기위해 입력값에 배열 중앙값을 더해서 반환
        return (byte)(centerOnBoard + _pos);
    }

    private ROOMTYPE NextRoom()
    {
        if (roomType == ROOMTYPE.HALLWAY)
        {
            return ROOMTYPE.GUEST;
        }
        else
        {
            return ROOMTYPE.HALLWAY;
        }
    }

    private byte RandomWallDir()
    {
        if (enterDir != null)
        {
            return (byte)enterDir;
        }
        else
        {
            return (byte)Random.Range(0, 4);
        }
    }

    private byte Randomsize()
    {
        //방 탑입에 따라 생성해야할 방의 수를 지정
        byte _value = 0;

        switch (roomType)
        {
            case ROOMTYPE.NPC:
                _value = (byte)Random.Range(3, 6);
                break;
            case ROOMTYPE.GUEST:
                _value = (byte)Random.Range(4, 5);
                break;
        }

        return _value;
    }
}
