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

    private int[,] mapboard = new int[256,256];

    private byte roomsize;

    private void Start()
    {
        byte size = (byte)Random.Range(2, 20);

        StartMap(0, 0, size);

        Debug.Log(size);
    }

    private void StartMap(int _x, int _y, byte _size)
    {
        int _xpos = _x + 127;
        int _ypos = _y + 127;

        GameObject room = Instantiate(roomprefab, transform.position, Quaternion.identity);
        
        for (byte i = 0; i < _size; i++)
        {
            ROOMDIR hallwaydir = (ROOMDIR)Random.Range(0, System.Enum.GetValues(typeof(ROOMDIR)).Length);

            if (i == 0)
            {
                mapboard[_xpos, _ypos] = (int)ROOMTYPE.HALLWAY;
                GameObject _floor = Instantiate(floorprefab, new Vector2(_x, _y), Quaternion.identity);
                _floor.transform.parent = room.transform;
            }
            else
            {
                switch (hallwaydir)
                {
                    case ROOMDIR.TOP:
                        if (mapboard[_xpos, _ypos + 1] != (int)ROOMTYPE.EMPTY)
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            mapboard[_xpos, _ypos + 1] = (int)ROOMTYPE.HALLWAY;
                            GameObject _floor = Instantiate(floorprefab, new Vector2(_x, _y + 1), Quaternion.identity);
                            _floor.transform.parent = room.transform;

                            _y++;
                        }
                        break;
                    case ROOMDIR.RIGHT:
                        if (mapboard[_xpos + 1, _ypos] != (int)ROOMTYPE.EMPTY)
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            mapboard[_xpos + 1, _ypos] = (int)ROOMTYPE.HALLWAY;
                            GameObject _floor = Instantiate(floorprefab, new Vector2(_x + 1, _y), Quaternion.identity);
                            _floor.transform.parent = room.transform;

                            _x++;
                        }
                        break;
                    case ROOMDIR.DOWN:
                        if (mapboard[_xpos - 1, _ypos] != (int)ROOMTYPE.EMPTY)
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            mapboard[_xpos - 1, _ypos] = (int)ROOMTYPE.HALLWAY;
                            GameObject _floor = Instantiate(floorprefab, new Vector2(_x - 1, _y), Quaternion.identity);
                            _floor.transform.parent = room.transform;

                            _x--;
                        }
                        break;
                    case ROOMDIR.LEFT:
                        if (mapboard[_xpos, _ypos - 1] != (int)ROOMTYPE.EMPTY)
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            mapboard[_xpos, _ypos - 1] = (int)ROOMTYPE.HALLWAY;
                            GameObject _floor = Instantiate(floorprefab, new Vector2(_x, _y - 1), Quaternion.identity);
                            _floor.transform.parent = room.transform;

                            _y--;
                        }
                        break;
                }
            }
        }
    }

    private void InstantiateHallWay(GameObject _Room, ROOMDIR _dir)
    {



        /*
        
        */
    }
}
