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
    private List<Vector3> nextFloorPosList = new List<Vector3>();    //다음으로 생성될 방의 좌표

    private GameObject roomPrefab;
    private GameObject floorPrefab;           //초기화 필요
    private GameObject wallPrefab;
    private GameObject doorPrefab;

    private byte centerOnBoard;               //배열의 중심인 (25,25)를 (0,0)으로 놓고 사용하기위한 변수
    private sbyte curX, curY;                 //바닥을 자식으로 둔 방의 좌표 = 1번째 바닥의 좌표
    private byte doorCount = 0;
    private byte doorMax = 4;

    void Start()
    {
        mapmanager = GameObject.Find("MapManager").GetComponent<MapManager>();
        centerOnBoard = (byte)(Mathf.Sqrt(mapmanager.RoomBoard.Length) / 2);

        roomPrefab = mapmanager.roomPrefab;

        if (roomType != ROOMTYPE.HALLWAY)
        {
            floorPrefab = mapmanager.floorPrefab;
        }
        else
        {
            floorPrefab = mapmanager.floor2Prefab;
        }
        
        wallPrefab = mapmanager.wallPrefab;
        doorPrefab = mapmanager.doorPrefab;

        curX = (sbyte)transform.position.x;
        curY = (sbyte)transform.position.y;

        roomDir = (DIRECTION)Random.Range(0, System.Enum.GetValues(typeof(DIRECTION)).Length);

        SpawnFloorProcess();
    }

    private void SpawnFloorProcess()
    {
        if (mapmanager.RoomBoard[PosParse(curX), PosParse(curY)] != null)
        {
            Destroy(gameObject);
        }

        if (roomType == ROOMTYPE.HALLWAY)
        {
            SpawnHallwayFloor();
        }
        else
        {
            SpawnRoomFloor();
            CreateRoomWall();
        }
    }

    private void SpawnRoomFloor()
    {
        if (mapmanager.curRoomCount < mapmanager.maxRoomCount && mapmanager.RoomBoard[PosParse(curX), PosParse(curY)] == null)
        {
            byte roomsize = RandomSize();                       //생성해야할 방의 타입을 확인한 후 그에 따라서 랜덤값 반환;
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

                    SetInfoToBoard(_floor, mapmanager.curRoomIndex);
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

                                SetInfoToBoard(_floor, mapmanager.curRoomIndex);                           //보드좌표에 생성된 바닥과 생성된 순서(방끼리 구분하기 위해)를 넣는다.

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

                                SetInfoToBoard(_floor, mapmanager.curRoomIndex);

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

                                SetInfoToBoard(_floor, mapmanager.curRoomIndex);

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

                                SetInfoToBoard(_floor, mapmanager.curRoomIndex);

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
            mapmanager.curRoomIndex++;
        }
    }

    private void CreateRoomWall()
    {
        DIRECTION randomdir = (DIRECTION)RandomWallDir();                               //만약 첫번째 방이 아닐 경우 이전방의 문 방향 값을 받아온
        byte percant = 4;                                               //문이 생길 확률 (1/percant)

        foreach (Vector2 _pos in floorPosList)                           //벽을 만들 바닥의 배열
        {
            WALLSTATE[] wallcheckarr = new WALLSTATE[4];                    //그 바닥의 4방향 벽상태

            foreach (Vector2 _checkpos in floorPosList)                  //같은 방의 바닥끼리 벽을 비우기 위해 비교할 바닥의 배열
            {
                for (DIRECTION i = randomdir; i < 4 + randomdir; i++)         //문이 달린 벽을 랜덤하게 생성해 주기 위한 순서
                {
                    switch (doorCount)                                   //문이 초반방에만 생기는 것을 막기 위해 확률을 낮게 만듬
                    {
                        //방의 사이즈에 따른 확률 변화가 필요할듯??
                        case 0: 
                            percant = 7;   //14%
                            break;
                        case 1:
                            percant = 13;   //8%
                            break;
                        case 2:
                            percant = 25;   //4%
                            break;
                        case 3:
                            percant = 50;    //2%
                            break;
                    }

                    if(i > DIRECTION.LEFT)
                    {
                        randomdir = i - 4;
                    }
                    else
                    {
                        randomdir = i;
                    }

                    switch (randomdir)
                    {
                        case DIRECTION.TOP:

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
                            else if (/*topInfo == null && */doorCount < doorMax)       //문이 4개를 넘지 않으면 일정 확률로 문만들고 방 생성
                            {
                                if (wallcheckarr[(byte)DIRECTION.TOP] == WALLSTATE.BLOCK)
                                {
                                    if (doorCount == 0)
                                    {
                                        wallcheckarr[(byte)DIRECTION.TOP] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector3(_pos.x, _pos.y + 1, (float)DIRECTION.BOTTOM));

                                        doorCount++;
                                    }
                                    else if (Random.Range(1, percant) == 1)
                                    {
                                        wallcheckarr[(byte)DIRECTION.TOP] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector3(_pos.x, _pos.y + 1, (float)DIRECTION.BOTTOM));

                                        doorCount++;
                                    }
                                }
                            }
                            break;
                        case DIRECTION.RIGHT:

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
                            else if (/*rightInfo == null && */doorCount < doorMax)
                            {
                                if (wallcheckarr[(byte)DIRECTION.RIGHT] == WALLSTATE.BLOCK)
                                {
                                    if (doorCount == 0)
                                    {
                                        wallcheckarr[(byte)DIRECTION.RIGHT] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector3(_pos.x + 1, _pos.y, (float)DIRECTION.LEFT));

                                        doorCount++;
                                    }
                                    else if (Random.Range(1, percant) == 1)
                                    {
                                        wallcheckarr[(byte)DIRECTION.RIGHT] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector3(_pos.x + 1, _pos.y, (float)DIRECTION.LEFT));

                                        doorCount++;
                                    }
                                }
                            }
                            break;
                        case DIRECTION.BOTTOM:

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
                            else if (/*borromInfo == null && */doorCount < doorMax)
                            {
                                if (wallcheckarr[(byte)DIRECTION.BOTTOM] == WALLSTATE.BLOCK)
                                {
                                    if (doorCount == 0)
                                    {
                                        wallcheckarr[(byte)DIRECTION.BOTTOM] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector3(_pos.x, _pos.y - 1, (float)DIRECTION.TOP));

                                        doorCount++;
                                    }
                                    else if (Random.Range(1, percant) == 1)
                                    {
                                        wallcheckarr[(byte)DIRECTION.BOTTOM] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector3(_pos.x, _pos.y - 1, (float)DIRECTION.TOP));

                                        doorCount++;
                                    }
                                }
                            }
                            break;
                        case DIRECTION.LEFT:

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
                            else if (/*leftInfo == null && */ doorCount < doorMax)
                            {
                                if (wallcheckarr[(byte)DIRECTION.LEFT] == WALLSTATE.BLOCK)
                                {
                                    if (doorCount == 0)
                                    {
                                        wallcheckarr[(byte)DIRECTION.LEFT] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector3(_pos.x - 1, _pos.y, (float)DIRECTION.RIGHT));

                                        doorCount++;
                                    }
                                    else if (Random.Range(1, percant) == 1)
                                    {
                                        wallcheckarr[(byte)DIRECTION.LEFT] = WALLSTATE.DOOR;

                                        nextFloorPosList.Add(new Vector3(_pos.x - 1, _pos.y, (float)DIRECTION.RIGHT));

                                        doorCount++;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            SpawnWall(wallcheckarr, _pos);
        }

        SpawnNextRoom(nextFloorPosList);
    }

    private void SpawnHallwayFloor()
    {
        WALLSTATE[] wallcheckarr = new WALLSTATE[4];

        if (enterDir != null)
        {
            switch (enterDir)
            {
                case DIRECTION.TOP:
                    if (mapmanager.RoomBoard[PosParse(curX), PosParse(curY - 1)] == null && mapmanager.RoomBoard[PosParse(curX), PosParse(curY - 2)] == null)
                    {
                        //복도의 첫번째 바닥
                        GameObject room_01 = Instantiate(floorPrefab, new Vector2(curX, curY), Quaternion.identity);
                        room_01.transform.parent = transform;
                        SetInfoToBoard(room_01, mapmanager.curRoomIndex);

                        wallcheckarr[(byte)DIRECTION.TOP] = WALLSTATE.DOOR;
                        wallcheckarr[(byte)DIRECTION.BOTTOM] = WALLSTATE.EMPTY;

                        SpawnWall(wallcheckarr, new Vector2(curX, curY));

                        //----------------------------------------------------------------------------------------------
                        wallcheckarr = new WALLSTATE[4];
                        //----------------------------------------------------------------------------------------------

                        //복도의 두번째 바닥
                        GameObject room_02 = Instantiate(floorPrefab, new Vector2(curX, curY - 1), Quaternion.identity);
                        room_02.transform.parent = transform;
                        SetInfoToBoard(room_02, mapmanager.curRoomIndex);

                        wallcheckarr[(byte)DIRECTION.BOTTOM] = WALLSTATE.DOOR;
                        wallcheckarr[(byte)DIRECTION.TOP] = WALLSTATE.EMPTY;

                        SpawnWall(wallcheckarr, new Vector2(curX, curY - 1));

                        mapmanager.curRoomIndex++;

                        nextFloorPosList.Add(new Vector3(curX, curY - 2, (float)DIRECTION.TOP));
                        SpawnNextRoom(nextFloorPosList);
                    }
                    else
                    {
                        //생성한 방의 문을 막힌벽으로 바꾸기
                        FloorInfo info = mapmanager.RoomBoard[PosParse(curX), PosParse(curY + 1)];
                        info.wallObjArr[(byte)DIRECTION.TOP] = wallPrefab;

                        Destroy(gameObject);
                    }
                    break;
                case DIRECTION.RIGHT:
                    if (mapmanager.RoomBoard[PosParse(curX - 1), PosParse(curY)] == null && mapmanager.RoomBoard[PosParse(curX - 2), PosParse(curY)] == null)
                    {
                        //복도의 첫번째 바닥
                        GameObject room_01 = Instantiate(floorPrefab, new Vector2(curX, curY), Quaternion.identity);
                        room_01.transform.parent = transform;
                        SetInfoToBoard(room_01, mapmanager.curRoomIndex);

                        wallcheckarr[(byte)DIRECTION.RIGHT] = WALLSTATE.DOOR;
                        wallcheckarr[(byte)DIRECTION.LEFT] = WALLSTATE.EMPTY;

                        SpawnWall(wallcheckarr, new Vector2(curX, curY));

                        //----------------------------------------------------------------------------------------------
                        wallcheckarr = new WALLSTATE[4];
                        //----------------------------------------------------------------------------------------------

                        //복도의 두번째 바닥
                        GameObject room_02 = Instantiate(floorPrefab, new Vector2(curX - 1, curY), Quaternion.identity);
                        room_02.transform.parent = transform;
                        SetInfoToBoard(room_02, mapmanager.curRoomIndex);

                        wallcheckarr[(byte)DIRECTION.LEFT] = WALLSTATE.DOOR;
                        wallcheckarr[(byte)DIRECTION.RIGHT] = WALLSTATE.EMPTY;

                        SpawnWall(wallcheckarr, new Vector2(curX - 1, curY));

                        mapmanager.curRoomIndex++;

                        nextFloorPosList.Add(new Vector3(curX - 2, curY, (float)DIRECTION.RIGHT));
                        SpawnNextRoom(nextFloorPosList);
                    }
                    else
                    {
                        //생성한 방의 문을 막힌벽으로 바꾸기
                        FloorInfo info = mapmanager.RoomBoard[PosParse(curX + 1), PosParse(curY)];
                        info.wallObjArr[(byte)DIRECTION.RIGHT] = wallPrefab;

                        Destroy(gameObject);
                    }
                    break;
                case DIRECTION.BOTTOM:
                    if (mapmanager.RoomBoard[PosParse(curX), PosParse(curY + 1)] == null && mapmanager.RoomBoard[PosParse(curX), PosParse(curY + 2)] == null)
                    {
                        //복도의 첫번째 바닥
                        GameObject room_01 = Instantiate(floorPrefab, new Vector2(curX, curY), Quaternion.identity);
                        room_01.transform.parent = transform;
                        SetInfoToBoard(room_01, mapmanager.curRoomIndex);

                        wallcheckarr[(byte)DIRECTION.BOTTOM] = WALLSTATE.DOOR;
                        wallcheckarr[(byte)DIRECTION.TOP] = WALLSTATE.EMPTY;

                        SpawnWall(wallcheckarr, new Vector2(curX, curY));

                        //----------------------------------------------------------------------------------------------
                        wallcheckarr = new WALLSTATE[4];
                        //----------------------------------------------------------------------------------------------

                        //복도의 두번째 바닥
                        GameObject room_02 = Instantiate(floorPrefab, new Vector2(curX, curY + 1), Quaternion.identity);
                        room_02.transform.parent = transform;
                        SetInfoToBoard(room_02, mapmanager.curRoomIndex);

                        wallcheckarr[(byte)DIRECTION.TOP] = WALLSTATE.DOOR;
                        wallcheckarr[(byte)DIRECTION.BOTTOM] = WALLSTATE.EMPTY;

                        SpawnWall(wallcheckarr, new Vector2(curX, curY + 1));

                        mapmanager.curRoomIndex++;

                        nextFloorPosList.Add(new Vector3(curX, curY + 2, (float)DIRECTION.BOTTOM));
                        SpawnNextRoom(nextFloorPosList);
                    }
                    else
                    {
                        //생성한 방의 문을 막힌벽으로 바꾸기
                        FloorInfo info = mapmanager.RoomBoard[PosParse(curX), PosParse(curY - 1)];
                        info.wallObjArr[(byte)DIRECTION.BOTTOM] = wallPrefab;

                        Destroy(gameObject);
                    }
                    break;
                case DIRECTION.LEFT:
                    if (mapmanager.RoomBoard[PosParse(curX + 1), PosParse(curY)] == null && mapmanager.RoomBoard[PosParse(curX + 2), PosParse(curY)] == null)
                    {
                        //복도의 첫번째 바닥
                        GameObject room_01 = Instantiate(floorPrefab, new Vector2(curX, curY), Quaternion.identity);
                        room_01.transform.parent = transform;
                        SetInfoToBoard(room_01, mapmanager.curRoomIndex);

                        wallcheckarr[(byte)DIRECTION.LEFT] = WALLSTATE.DOOR;
                        wallcheckarr[(byte)DIRECTION.RIGHT] = WALLSTATE.EMPTY;

                        SpawnWall(wallcheckarr, new Vector2(curX, curY));

                        //----------------------------------------------------------------------------------------------
                        wallcheckarr = new WALLSTATE[4];
                        //----------------------------------------------------------------------------------------------

                        //복도의 두번째 바닥
                        GameObject room_02 = Instantiate(floorPrefab, new Vector2(curX + 1, curY), Quaternion.identity);
                        room_02.transform.parent = transform;
                        SetInfoToBoard(room_02, mapmanager.curRoomIndex);

                        wallcheckarr[(byte)DIRECTION.RIGHT] = WALLSTATE.DOOR;
                        wallcheckarr[(byte)DIRECTION.LEFT] = WALLSTATE.EMPTY;

                        SpawnWall(wallcheckarr, new Vector2(curX + 1, curY));

                        mapmanager.curRoomIndex++;

                        nextFloorPosList.Add(new Vector3(curX + 2, curY, (float)DIRECTION.LEFT));
                        SpawnNextRoom(nextFloorPosList);
                    }
                    else
                    {
                        //생성한 방의 문을 막힌벽으로 바꾸기
                        FloorInfo info = mapmanager.RoomBoard[PosParse(curX - 1), PosParse(curY)];
                        info.wallObjArr[(byte)DIRECTION.LEFT] = wallPrefab;

                        Destroy(gameObject);
                    }
                    break;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SpawnNextRoom(List<Vector3> _vec3list)
    {
        StartCoroutine(Delay(_vec3list));
    }

    IEnumerator Delay(List<Vector3> _vec3list)
    {
        yield return new WaitForSeconds(1.0f);

        ROOMTYPE _roomtype = 0;

        if (roomType != ROOMTYPE.HALLWAY)
        {
            _roomtype = ROOMTYPE.HALLWAY;
        }
        else
        {
            _roomtype = ROOMTYPE.GUEST;
        }

        foreach (Vector3 _vec3 in _vec3list)
        {
            Vector2 _pos = new Vector2(_vec3.x, _vec3.y);
            DIRECTION _dir = (DIRECTION)_vec3.z;

            GameObject room = Instantiate(roomPrefab, _pos, Quaternion.identity);
            RoomController roomcontroller = room.transform.GetComponent<RoomController>();
            roomcontroller.roomType = _roomtype;
            roomcontroller.enterDir = _dir;
        }
    }

    private void SpawnWall(WALLSTATE[] _wallstatearr, Vector2 _pos)
    {
        byte spawndir = 0;

        foreach (WALLSTATE wallstate in _wallstatearr)
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
            else if (wallstate == WALLSTATE.DOOR)
            {
                Instantiate(doorPrefab, wallpos, Quaternion.Euler(0, 0, wallrotaion)).transform.parent = floor.transform;
            }

            spawndir++;
        }
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

    private byte RandomSize()
    {
        //방 탑입에 따라 생성해야할 방의 수를 지정
        byte _value = 0;

        switch (roomType)
        {
            case ROOMTYPE.NPC:
                _value = (byte)Random.Range(3, 6);
                break;
            case ROOMTYPE.GUEST:
                _value = (byte)Random.Range(1, 5);
                break;
        }

        return _value;
    }
}