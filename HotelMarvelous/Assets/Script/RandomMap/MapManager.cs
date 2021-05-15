using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ROOMTYPE : byte
{
    EMPTY,
    HALLWAY,
    GUEST,
    NPC
}

public enum ROOMDIR
{
    TOP,
    RIGHT,
    DOWN,
    LEFT
}

public class MapManager : MonoBehaviour
{
    public GameObject roomprefab;       //임시 나중에 리소스에서 불러오는것 잊지마요!!
    public GameObject floorprefab;

    public ROOMTYPE roomtype;

    public List<GameObject> TESTROOMLIST = new List<GameObject>();

    private int[,] mapboard = new int[256, 256];
    public int Roomcount = 5;

    public bool isfirst = true;

    public void InstiateRoom(int _x, int _y, ROOMTYPE _roomtype)
    {
        int _xpos = _x + 127;
        int _ypos = _y + 127;
        byte _size = RoomSize();
        

        Debug.Log(_size);
        Roomcount--;

        if(Roomcount < 1)
        {
            return;
        }

        GameObject room = Instantiate(roomprefab, new Vector3(_x, _y, 0), Quaternion.identity);
        RoomInfo roominfo = room.GetComponent<RoomInfo>();

        if (isfirst)
        {
            isfirst = false;
            roominfo.roomtype = ROOMTYPE.HALLWAY;
        }
        else
        {
            roominfo.roomtype = _roomtype;
        }

        TESTROOMLIST.Add(room);

        for (byte i = 0; i < _size; i++)
        {
            ROOMDIR hallwaydir = (ROOMDIR)Random.Range(0, System.Enum.GetValues(typeof(ROOMDIR)).Length);

            if (i == 0)
            {
                mapboard[_xpos, _ypos] = (byte)_roomtype;

                GameObject _floor = Instantiate(floorprefab, new Vector3(_x, _y, 0), Quaternion.identity);
                _floor.transform.parent = room.transform;

                roominfo.SetFloorList(_floor);
            }
            else
            {
                switch (hallwaydir)
                {
                    case ROOMDIR.TOP:
                        if (mapboard[_xpos, _ypos + 1] != (byte)ROOMTYPE.EMPTY)
                        {
                            if (i > 1)
                            {
                                i--;
                            }
                            break;
                        }
                        else
                        {
                            mapboard[_xpos, _ypos + 1] = (byte)_roomtype;

                            GameObject _floor = Instantiate(floorprefab, new Vector3(_x, _y + 1, 0), Quaternion.identity);
                            _floor.transform.parent = room.transform;

                            roominfo.SetFloorList(_floor);

                            _y++;
                            _ypos++;
                        }
                        break;
                    case ROOMDIR.RIGHT:
                        if (mapboard[_xpos + 1, _ypos] != (byte)ROOMTYPE.EMPTY)
                        {
                            if (i > 1)
                            {
                                i--;
                            }
                            break;
                        }
                        else
                        {
                            mapboard[_xpos + 1, _ypos] = (byte)_roomtype;

                            GameObject _floor = Instantiate(floorprefab, new Vector3(_x + 1, _y, 0), Quaternion.identity);
                            _floor.transform.parent = room.transform;

                            roominfo.SetFloorList(_floor);

                            _x++;
                            _xpos++;
                        }
                        break;
                    case ROOMDIR.DOWN:
                        if (mapboard[_xpos - 1, _ypos] != (byte)ROOMTYPE.EMPTY)
                        {
                            if (i > 1)
                            {
                                i--;
                            }
                            break;
                        }
                        else
                        {
                            mapboard[_xpos - 1, _ypos] = (byte)_roomtype;

                            GameObject _floor = Instantiate(floorprefab, new Vector3(_x - 1, _y, 0), Quaternion.identity);
                            _floor.transform.parent = room.transform;

                            roominfo.SetFloorList(_floor);

                            _x--;
                            _xpos--;
                        }
                        break;
                    case ROOMDIR.LEFT:
                        if (mapboard[_xpos, _ypos - 1] != (byte)ROOMTYPE.EMPTY)
                        {
                            if (i > 1)
                            {
                                i--;
                            }
                            break;
                        }
                        else
                        {
                            mapboard[_xpos, _ypos - 1] = (byte)_roomtype;

                            GameObject _floor = Instantiate(floorprefab, new Vector3(_x, _y - 1, 0), Quaternion.identity);
                            _floor.transform.parent = room.transform;

                            roominfo.SetFloorList(_floor);

                            _y--;
                            _ypos--;
                        }
                        break;
                }
            }
        }

        roominfo.CheckFourDir();
    }

    private byte RoomSize()
    {
        byte _i = 0;

        switch (roomtype)
        {
            case ROOMTYPE.GUEST:
                _i = (byte)Random.Range(1, 5);
                break;
            case ROOMTYPE.HALLWAY:
                _i = 2;
                break;
        }
        return _i;
    }

    public int[,] GetMapBoard()
    {
        return mapboard;
    }

    public void OnclickMapSpawn()
    {
        Roomcount = 5;
        InstiateRoom(0, 0, roomtype);
    }

    public void OnclickMadelete()
    {
        foreach (GameObject go in TESTROOMLIST)
        {
            Destroy(go);
        }

        TESTROOMLIST.Clear();
        mapboard = new int[256, 256];
    }
}
