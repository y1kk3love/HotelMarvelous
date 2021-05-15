using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    private MapManager mapmanager;

    private List<GameObject> floorList = new List<GameObject>();        //같은방에 생성된 바닥오브젝트들의 리스트

    public ROOMTYPE roomtype;

    public GameObject blockWall;
    public GameObject doorWall;

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
                if (new Vector2(floorPos.x + 1, floorPos.y) == _pos && !emptywallarr[0])
                {
                    emptywallarr[0] = true;
                }
                if (new Vector2(floorPos.x, floorPos.y + 1) == _pos && !emptywallarr[1])
                {
                    emptywallarr[1] = true;
                }
                if (new Vector2(floorPos.x - 1, floorPos.y) == _pos && !emptywallarr[2])
                {
                    emptywallarr[2] = true;
                }
                if (new Vector2(floorPos.x, floorPos.y - 1) == _pos && !emptywallarr[3])
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
                        case 0:
                            if(mapmanager.GetMapBoard()[(int)floorPos.x + 128, (int)floorPos.y + 127] == (byte)ROOMTYPE.EMPTY && doorpercent == 1 && maxdoor < 4)
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
                        case 1:
                            if (mapmanager.GetMapBoard()[(int)floorPos.x + 127, (int)floorPos.y + 128] == (byte)ROOMTYPE.EMPTY && doorpercent == 1 && maxdoor < 4)
                            {
                                maxdoor++;
                                doorposList.Add(new Vector2(floorPos.x, floorPos.y + 1));
                                Instantiate(doorWall, new Vector3(floorPos.x, floorPos.y + 0.45f, -0.05f), Quaternion.Euler(0, 0, 90)).transform.parent = _floor.transform;
                            }
                            else
                            {
                                Instantiate(blockWall, new Vector3(floorPos.x, floorPos.y + 0.45f, -0.05f), Quaternion.Euler(0, 0, 90)).transform.parent = _floor.transform;
                            }
                            break;
                        case 2:
                            if (mapmanager.GetMapBoard()[(int)floorPos.x + 126, (int)floorPos.y + 127] == (byte)ROOMTYPE.EMPTY && doorpercent == 1 && maxdoor < 4)
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
                        case 3:
                            if (mapmanager.GetMapBoard()[(int)floorPos.x + 127, (int)floorPos.y + 126] == (byte)ROOMTYPE.EMPTY && doorpercent == 1 && maxdoor < 4)
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
                    }
                }
            }
        }

        foreach(Vector2 passpos in doorposList)
        {
            if (roomtype != ROOMTYPE.HALLWAY)
            {
                mapmanager.InstiateRoom((int)passpos.x, (int)passpos.y, ROOMTYPE.HALLWAY);
            }
            else
            {
                //ROOMTYPE randomroom = (ROOMTYPE)Random.Range(2, System.Enum.GetValues(typeof(ROOMTYPE)).Length);
                ROOMTYPE randomroom = ROOMTYPE.GUEST;
                mapmanager.InstiateRoom((int)passpos.x, (int)passpos.y, randomroom);
            }
        }
    }

    public void SetFloorList(GameObject _floor)
    {
        floorList.Add(_floor);
    }
}
