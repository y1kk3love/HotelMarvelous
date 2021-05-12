using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ROOMTYPE
{
    EMPTY,
    GUEST,
    HALLWAY,
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


    private int[,] mapboard = new int[256, 256];
    private Vector2[] roomboard = new Vector2[20];

    public byte roomsize;

    private void Start()
    {
        StartMap(0, 0, roomsize);
    }

    private void StartMap(int _x, int _y, byte _size)
    {
        int _xpos = _x + 127;
        int _ypos = _y + 127;

        GameObject room = Instantiate(roomprefab, new Vector3(_x, _y, 0), Quaternion.identity);
        RoomInfo roominfo = room.GetComponent<RoomInfo>();

        for (byte i = 0; i < _size; i++)
        {
            ROOMDIR hallwaydir = (ROOMDIR)Random.Range(0, System.Enum.GetValues(typeof(ROOMDIR)).Length);

            if (i == 0)
            {
                mapboard[_xpos, _ypos] = (int)ROOMTYPE.HALLWAY;
                roomboard[i] = new Vector2(_x, _y);

                GameObject _floor = Instantiate(floorprefab, new Vector3(_x, _y, 0), Quaternion.identity);
                _floor.transform.parent = room.transform;

                roominfo.SetFloorList(_floor);
            }
            else
            {
                switch (hallwaydir)
                {
                    case ROOMDIR.TOP:
                        if (mapboard[_xpos, _ypos + 1] != (int)ROOMTYPE.EMPTY)
                        {
                            if (i > 1)
                            {
                                i--;
                            }
                            break;
                        }
                        else
                        {
                            mapboard[_xpos, _ypos + 1] = (int)ROOMTYPE.HALLWAY;
                            roomboard[i] = new Vector2(_x, _y + 1);

                            GameObject _floor = Instantiate(floorprefab, new Vector3(_x, _y + 1, 0), Quaternion.identity);
                            _floor.transform.parent = room.transform;

                            roominfo.SetFloorList(_floor);

                            _y++;
                            _ypos++;
                        }
                        break;
                    case ROOMDIR.RIGHT:
                        if (mapboard[_xpos + 1, _ypos] != (int)ROOMTYPE.EMPTY)
                        {
                            if (i > 1)
                            {
                                i--;
                            }
                            break;
                        }
                        else
                        {
                            mapboard[_xpos + 1, _ypos] = (int)ROOMTYPE.HALLWAY;
                            roomboard[i] = new Vector2(_x + 1, _y);

                            GameObject _floor = Instantiate(floorprefab, new Vector3(_x + 1, _y, 0), Quaternion.identity);
                            _floor.transform.parent = room.transform;

                            roominfo.SetFloorList(_floor);

                            _x++;
                            _xpos++;
                        }
                        break;
                    case ROOMDIR.DOWN:
                        if (mapboard[_xpos - 1, _ypos] != (int)ROOMTYPE.EMPTY)
                        {
                            if (i > 1)
                            {
                                i--;
                            }
                            break;
                        }
                        else
                        {
                            mapboard[_xpos - 1, _ypos] = (int)ROOMTYPE.HALLWAY;
                            roomboard[i] = new Vector2(_x - 1, _y);

                            GameObject _floor = Instantiate(floorprefab, new Vector3(_x - 1, _y, 0), Quaternion.identity);
                            _floor.transform.parent = room.transform;

                            roominfo.SetFloorList(_floor);

                            _x--;
                            _xpos--;
                        }
                        break;
                    case ROOMDIR.LEFT:
                        if (mapboard[_xpos, _ypos - 1] != (int)ROOMTYPE.EMPTY)
                        {
                            if (i > 1)
                            {
                                i--;
                            }
                            break;
                        }
                        else
                        {
                            mapboard[_xpos, _ypos - 1] = (int)ROOMTYPE.HALLWAY;
                            roomboard[i] = new Vector2(_x, _y - 1);

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

        roominfo.CheckFourDir(roomboard);
    }

    public int[,] GetMapBoard()
    {
        return mapboard;
    }

    private void InstantiateHallWay(GameObject _Room, ROOMDIR _dir)
    {

    }
}
