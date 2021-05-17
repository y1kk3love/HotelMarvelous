using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    private MapManager mapmanager;

    private List<GameObject> floorList = new List<GameObject>();        //같은방에 생성된 바닥오브젝트들의 리스트

    public ROOMTYPE roomType;
    private ROOMDIR waydir;

    private GameObject floorPrefab;

    public GameObject blockWall;
    public GameObject doorWall;

    public byte _size;
    void Start()
    {
        mapmanager = GameObject.Find("MapManager").GetComponent<MapManager>();
        waydir = (ROOMDIR)Random.Range(0, System.Enum.GetValues(typeof(ROOMDIR)).Length);
        floorPrefab = mapmanager.floorprefab;     //임시

        InstiateRoom();
        InfiniteLoopChecker.Reset();
    }

    public void InstiateRoom()
    {
        int curX = (int)transform.position.x;
        int curY = (int)transform.position.y;
        int boardX = curX + 25;
        int boardY = curY + 25;
        byte blockcount = 0;
        _size = RoomSize();

        for (byte i = 0; i < _size; i++)
        {
            if(blockcount > 2)
            {
                i = _size;
            }
            else
            {
                blockcount = 0;
            }

            if (i == 0)
            {
                mapmanager.mapboard[boardX, boardY] = (byte)roomType;

                GameObject _floor = Instantiate(floorPrefab, new Vector3(curX, curY, 0), Quaternion.identity);
                _floor.transform.parent = transform;

                floorList.Add(_floor);
            }
            else
            {
                InfiniteLoopChecker.Check();

                switch (waydir)
                {
                    case ROOMDIR.TOP:
                        if (mapmanager.mapboard[boardX, boardY + 1] == (byte)ROOMTYPE.EMPTY)
                        {
                            mapmanager.mapboard[boardX, boardY + 1] = (byte)roomType;

                            GameObject _floor = Instantiate(floorPrefab, new Vector3(curX, curY + 1, 0), Quaternion.identity);
                            _floor.transform.parent = transform;

                            floorList.Add(_floor);

                            curY++;
                            boardY++;
                        }
                        else
                        {
                            if (i > 1)
                            {
                                //i--;
                                blockcount++;
                            }
                        }
                        break;
                    case ROOMDIR.RIGHT:
                        if (mapmanager.mapboard[boardX + 1, boardY] == (byte)ROOMTYPE.EMPTY)
                        {
                            mapmanager.mapboard[boardX + 1, boardY] = (byte)roomType;

                            GameObject _floor = Instantiate(floorPrefab, new Vector3(curX + 1, curY, 0), Quaternion.identity);
                            _floor.transform.parent = transform;

                            floorList.Add(_floor);

                            curX++;
                            boardX++;
                        }
                        else
                        {
                            if (i > 1)
                            {
                                //i--;
                                blockcount++;
                            }
                        }
                        break;
                    case ROOMDIR.BOTTOM:
                        if (mapmanager.mapboard[boardX - 1, boardY] == (byte)ROOMTYPE.EMPTY)
                        {
                            mapmanager.mapboard[boardX - 1, boardY] = (byte)roomType;

                            GameObject _floor = Instantiate(floorPrefab, new Vector3(curX - 1, curY, 0), Quaternion.identity);
                            _floor.transform.parent = transform;

                            floorList.Add(_floor);

                            curX--;
                            boardX--;
                        }
                        else
                        {
                            if (i > 1)
                            {
                                //i--;
                                blockcount++;
                            }
                        }
                        break;
                    case ROOMDIR.LEFT:
                        if (mapmanager.mapboard[boardX, boardY - 1] == (byte)ROOMTYPE.EMPTY)
                        {
                            mapmanager.mapboard[boardX, boardY - 1] = (byte)roomType;

                            GameObject _floor = Instantiate(floorPrefab, new Vector3(curX, curY - 1, 0), Quaternion.identity);
                            _floor.transform.parent = transform;

                            floorList.Add(_floor);

                            curY--;
                            boardY--;
                        }
                        else
                        {
                            if (i > 1)
                            {
                                //i--;
                                blockcount++;
                            }
                        }
                        break;
                }
            }

            LoopDir();
        }

        CheckFourDir();
    }

    public void CheckFourDir()
    {
        mapmanager = GameObject.Find("MapManager").GetComponent<MapManager>();

        List<Vector2> doorposList = new List<Vector2>();
        byte maxdoor = 0;

        foreach (GameObject _floor in floorList)
        {
            bool[] emptywallarr = new bool[4];
            Vector2 floorPos = _floor.transform.position;

            foreach (GameObject _curpos in floorList)
            {
                Vector2 _pos = _curpos.transform.position;

                if(_pos == floorPos)
                {
                    continue;
                }
                if (new Vector2(floorPos.x, floorPos.y + 1) == _pos && !emptywallarr[(byte)ROOMDIR.TOP])
                {
                    emptywallarr[0] = true;
                }
                if (new Vector2(floorPos.x + 1, floorPos.y) == _pos && !emptywallarr[(byte)ROOMDIR.RIGHT])
                {
                    emptywallarr[1] = true;
                }
                if (new Vector2(floorPos.x, floorPos.y - 1) == _pos && !emptywallarr[(byte)ROOMDIR.BOTTOM])
                {
                    emptywallarr[2] = true;
                }
                if (new Vector2(floorPos.x - 1, floorPos.y) == _pos && !emptywallarr[(byte)ROOMDIR.LEFT])
                {
                    emptywallarr[3] = true;
                }
            }

            for(int i = 0; i < 4; i++)
            {
                if (!emptywallarr[i])
                {
                    byte doorpercent = (byte)Random.Range(1, 3);

                    switch (i)
                    {
                        case (byte)ROOMDIR.TOP:
                            if (mapmanager.mapboard[(int)floorPos.x + 25, (int)floorPos.y + 26] == (byte)ROOMTYPE.EMPTY && doorpercent == 1 && maxdoor < 4)
                            {
                                maxdoor++;
                                doorposList.Add(new Vector2(floorPos.x, floorPos.y + 1));
                                Instantiate(doorWall, new Vector3(floorPos.x, floorPos.y + 0.45f, -0.05f), Quaternion.Euler(0, 0, -90)).transform.parent = _floor.transform;
                            }
                            else
                            {
                                Instantiate(blockWall, new Vector3(floorPos.x, floorPos.y + 0.45f, -0.05f), Quaternion.Euler(0, 0, -90)).transform.parent = _floor.transform;
                            }
                            break;
                        case (byte)ROOMDIR.RIGHT:
                            if(mapmanager.mapboard[(int)floorPos.x + 26, (int)floorPos.y + 25] == (byte)ROOMTYPE.EMPTY && doorpercent == 1 && maxdoor < 4)
                            {
                                maxdoor++;
                                doorposList.Add(new Vector2(floorPos.x + 1, floorPos.y));
                                Instantiate(doorWall, new Vector3(floorPos.x + 0.45f, floorPos.y, -0.05f), Quaternion.Euler(0, 0, 0)).transform.parent = _floor.transform;
                            }
                            else
                            {
                                Instantiate(blockWall, new Vector3(floorPos.x + 0.45f, floorPos.y, -0.05f), Quaternion.Euler(0, 0, 0)).transform.parent = _floor.transform;
                            }
                            break;
                        case (byte)ROOMDIR.BOTTOM:
                            if (mapmanager.mapboard[(int)floorPos.x + 25, (int)floorPos.y + 24] == (byte)ROOMTYPE.EMPTY && doorpercent == 1 && maxdoor < 4)
                            {
                                maxdoor++;
                                doorposList.Add(new Vector2(floorPos.x, floorPos.y - 1));
                                Instantiate(doorWall, new Vector3(floorPos.x, floorPos.y - 0.45f, -0.05f), Quaternion.Euler(0, 0, -90)).transform.parent = _floor.transform;
                            }
                            else
                            {
                                Instantiate(blockWall, new Vector3(floorPos.x, floorPos.y - 0.45f, -0.05f), Quaternion.Euler(0, 0, -90)).transform.parent = _floor.transform;
                            }
                            break;
                        case (byte)ROOMDIR.LEFT:
                            if (mapmanager.mapboard[(int)floorPos.x + 24, (int)floorPos.y + 25] == (byte)ROOMTYPE.EMPTY && doorpercent == 1 && maxdoor < 4)
                            {
                                maxdoor++;
                                doorposList.Add(new Vector2(floorPos.x - 1, floorPos.y));
                                Instantiate(doorWall, new Vector3(floorPos.x - 0.45f, floorPos.y, -0.05f), Quaternion.Euler(0, 0, -180)).transform.parent = _floor.transform;
                            }
                            else
                            {
                                Instantiate(blockWall, new Vector3(floorPos.x - 0.45f, floorPos.y, -0.05f), Quaternion.Euler(0, 0, -180)).transform.parent = _floor.transform;
                            }
                            break;
                    }
                }
            }
        }

        foreach (Vector2 passpos in doorposList)
        {
            if (mapmanager.curRoomCount > mapmanager.maxRoomCount - 1)
            {
                return;
            }
            else
            {
                mapmanager.curRoomCount++;

                if (roomType != ROOMTYPE.HALLWAY)
                {
                    //StartCoroutine(InstantiateRoom(passpos, ROOMTYPE.HALLWAY));
                    mapmanager.needRoomPosList.Add(new Vector3(passpos.x, passpos.y, (byte)ROOMTYPE.HALLWAY));
                }
                else
                {
                    //ROOMTYPE randomroom = (ROOMTYPE)Random.Range(2, System.Enum.GetValues(typeof(ROOMTYPE)).Length);

                    //StartCoroutine(InstantiateRoom(passpos, ROOMTYPE.GUEST));
                    mapmanager.needRoomPosList.Add(new Vector3(passpos.x, passpos.y, (byte)ROOMTYPE.GUEST));
                }
            }
        }
    }

    IEnumerator InstantiateRoom(Vector2 _pos, ROOMTYPE _type)
    {
        yield return new WaitForSeconds(1f);

        GameObject room = Instantiate(mapmanager.roomprefab, _pos, Quaternion.identity);
        room.transform.GetComponent<RoomInfo>().roomType = _type;

        mapmanager.instRoomList.Add(room);
    }

    private byte RoomSize()
    {
        byte _i = 0;

        switch (roomType)
        {
            case ROOMTYPE.GUEST:
                //_i = (byte)Random.Range(1, 6);
                _i = 4;
                break;
            case ROOMTYPE.HALLWAY:
                _i = 2;
                break;
        }
        return _i;
    }

    private void LoopDir()
    {
        waydir++;

        if (waydir > ROOMDIR.LEFT)
        {
            waydir = 0;
        }
    }
}

public static class InfiniteLoopChecker
{
    private static int infiniteLoopNum = 0;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Reset() => infiniteLoopNum = 0;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Check(int maxLoopNumber = 10000)
    {
        if (infiniteLoopNum++ > maxLoopNumber)
            throw new System.Exception("Infinite Loop Detected.");
    }
}
