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
    private byte curX, curY;
    private byte doorCount = 0;


    void Start()
    {
        mapmanager = GameObject.Find("MapManager").GetComponent<MapManager>();
        centerOnBoard = (byte)(Mathf.Sqrt(mapmanager.RoomBoard.Length) / 2);

        floorPrefab = mapmanager.floorPrefab;
        wallPrefab = mapmanager.wallPrefab;
        doorPrefab = mapmanager.doorPrefab;

        curX = (byte)transform.position.x;
        curY = (byte)transform.position.y;

        roomDir = (DIRECTION)Random.Range(0, System.Enum.GetValues(typeof(DIRECTION)).Length);

        SpawnRoomFloor();
        SpawnFloorWall();
    }

    private void SpawnRoomFloor()
    {
        if (mapmanager.curRoomCount < mapmanager.maxRoomCount)
        {
            byte roomsize = Randomsize();                       //생성해야할 방의 타입을 확인한 후 그에 따라서 랜덤값 반환;

            for (int i = 0; i < roomsize; i++)
            {
                if (i == 0)                                      //첫 타일은 방의 좌표와 같은곳에 생성
                {
                    GameObject _floor = Instantiate(floorPrefab, new Vector2(curX, curY), Quaternion.identity);
                    _floor.transform.parent = transform;

                    SetInfoToBoard(new Vector2(curX, curY), _floor, mapmanager.curRoomCount);
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

                                SetInfoToBoard(new Vector2(curX, curY + 1), _floor, mapmanager.curRoomCount);                           //보드좌표에 생성된 바닥과 생성된 순서(방끼리 구분하기 위해)를 넣는다.

                                curY++;             //생성된 경우 좌표값을 변경해줍니다.
                                roomDir = 0;        //방향 카운터를 초기화
                            }
                            else
                            {
                                roomDir++;          //바닥 생성이 불가능할 경우 방향 카운터++
                            }
                            break;
                        case DIRECTION.RIGHT:
                            if (mapmanager.RoomBoard[PosParse(curX + 1), PosParse(curY)] == null)
                            {
                                GameObject _floor = Instantiate(floorPrefab, new Vector2(curX + 1, curY), Quaternion.identity);
                                _floor.transform.parent = transform;

                                SetInfoToBoard(new Vector2(curX + 1, curY), _floor, mapmanager.curRoomCount);

                                curX++;
                                roomDir = 0;
                            }
                            else
                            {
                                roomDir++;
                            }
                            break;
                        case DIRECTION.BOTTOM:
                            if (mapmanager.RoomBoard[PosParse(curX), PosParse(curY - 1)] == null)
                            {
                                GameObject _floor = Instantiate(floorPrefab, new Vector2(curX, curY - 1), Quaternion.identity);
                                _floor.transform.parent = transform;

                                SetInfoToBoard(new Vector2(curX, curY - 1), _floor, mapmanager.curRoomCount);

                                curY--;
                                roomDir = 0;
                            }
                            else
                            {
                                roomDir++;
                            }
                            break;
                        case DIRECTION.LEFT:
                            if (mapmanager.RoomBoard[PosParse(curX - 1), PosParse(curY)] == null)
                            {
                                GameObject _floor = Instantiate(floorPrefab, new Vector2(curX - 1, curY), Quaternion.identity);
                                _floor.transform.parent = transform;

                                SetInfoToBoard(new Vector2(curX - 1, curY), _floor, mapmanager.curRoomCount);

                                curX--;
                                roomDir = 0;
                            }
                            else
                            {
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
        WALLSTATE[] wallcheckarr = new WALLSTATE[4];                    //그 바닥의 4방향 벽상태

        byte randomdir = RandomWallDir();                               //만약 첫번째 방이 아닐 경우 이전방의 문 방향 값을 받아온다
        byte percant = 3;                                               //문이 생길 확률 (1/percant)

        foreach (Vector2 _pos in floorPosList)                           //벽을 만들 바닥의 배열
        {
            foreach (Vector2 _checkpos in floorPosList)                  //같은 방의 바닥끼리 벽을 비우기 위해 비교할 바닥의 배열
            {
                for (int i = randomdir; i < 4 + randomdir; i++)         //문이 달린 벽을 랜덤하게 생성해 주기 위한 순서
                {
                    switch (Mathf.Abs(i - 4))
                    {
                        case (byte)DIRECTION.TOP:

                            FloorInfo topInfo = mapmanager.RoomBoard[PosParse(_pos.x), PosParse(_pos.y + 1)];

                            if (new Vector2(_pos.x, _pos.y + 1) == _checkpos)           //확인한 칸에 이미 같은 방의 바닥이 있으면 벽을 비운다.
                            {
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
                            else if (topInfo != null && topInfo.roomType != ROOMTYPE.HALLWAY)
                            {       //확인한 칸에 이미 바닥이 있고, 그 바닥의 방타입이 복도가 아닐때
                                wallcheckarr[(byte)DIRECTION.TOP] = WALLSTATE.DOOR;

                                topInfo.wallObj[(byte)DIRECTION.BOTTOM] = doorPrefab;

                                doorCount++;
                            }
                            break;
                        case (byte)DIRECTION.RIGHT:

                            FloorInfo rightInfo = mapmanager.RoomBoard[PosParse(_pos.x + 1), PosParse(_pos.y)];

                            if (new Vector2(_pos.x + 1, _pos.y) == _checkpos)
                            {
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
                            else if (rightInfo != null && rightInfo.roomType != ROOMTYPE.HALLWAY)
                            {
                                wallcheckarr[(byte)DIRECTION.RIGHT] = WALLSTATE.DOOR;

                                rightInfo.wallObj[(byte)DIRECTION.LEFT] = doorPrefab;

                                doorCount++;
                            }
                            break;
                        case (byte)DIRECTION.BOTTOM:

                            FloorInfo borromInfo = mapmanager.RoomBoard[PosParse(_pos.x), PosParse(_pos.y - 1)];

                            if (new Vector2(_pos.x, _pos.y - 1) == _checkpos)
                            {
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
                            else if (borromInfo != null && borromInfo.roomType != ROOMTYPE.HALLWAY)
                            {
                                wallcheckarr[(byte)DIRECTION.BOTTOM] = WALLSTATE.DOOR;

                                borromInfo.wallObj[(byte)DIRECTION.TOP] = doorPrefab;

                                doorCount++;
                            }
                            break;
                        case (byte)DIRECTION.LEFT:
                            if (new Vector2(_pos.x - 1, _pos.y) == _checkpos)
                            {
                                wallcheckarr[(byte)DIRECTION.LEFT] = WALLSTATE.EMPTY;
                            }

                            FloorInfo leftInfo = mapmanager.RoomBoard[PosParse(_pos.x - 1), PosParse(_pos.y)];

                            if (new Vector2(_pos.x - 1, _pos.y) == _checkpos)
                            {
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
                            else if (leftInfo != null && leftInfo.roomType != ROOMTYPE.HALLWAY)
                            {
                                wallcheckarr[(byte)DIRECTION.LEFT] = WALLSTATE.DOOR;

                                leftInfo.wallObj[(byte)DIRECTION.RIGHT] = doorPrefab;

                                doorCount++;
                            }
                            break;
                    }
                }

            }
        }
    }

    private void SetInfoToBoard(Vector2 _pos, GameObject _roomobj, byte _index)
    {
        FloorInfo floorinfo = new FloorInfo();

        floorinfo.floorPos = _pos;
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
                _value = (byte)Random.Range(1, 4);
                break;
        }

        return _value;
    }
}
