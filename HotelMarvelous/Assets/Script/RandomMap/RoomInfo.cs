using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    private MapManager mapmanager;

    private List<GameObject> floorList = new List<GameObject>();

    public GameObject blockWall;
    public GameObject doorWall;

    void Start()
    {
        mapmanager = GameObject.Find("MapManager").GetComponent<MapManager>();
    }

    public void CheckFourDir(Vector2[] _roomarr)
    {
        foreach (GameObject _floor in floorList)
        {
            bool[] needwallarr = new bool[4];
            Vector2 floorPos = _floor.transform.position;

            foreach (Vector2 _pos in _roomarr)
            {
                if(_pos == floorPos)
                {
                    continue;
                }
                if (new Vector2(floorPos.x + 1, floorPos.y) == _pos && !needwallarr[0])
                {
                    needwallarr[0] = true;
                }
                if (new Vector2(floorPos.x, floorPos.y + 1) == _pos && !needwallarr[1])
                {
                    needwallarr[1] = true;
                }
                if (new Vector2(floorPos.x - 1, floorPos.y) == _pos && !needwallarr[2])
                {
                    needwallarr[2] = true;
                }
                if (new Vector2(floorPos.x, floorPos.y - 1) == _pos && !needwallarr[3])
                {
                    needwallarr[3] = true;
                }
            }

            for(int i = 0; i < 4; i++)
            {
                if (!needwallarr[i])
                {
                    switch (i)
                    {
                        case 0:
                            Instantiate(blockWall, new Vector3(floorPos.x + 0.45f, floorPos.y, -0.05f), Quaternion.Euler(0, 0, 0)).transform.parent = _floor.transform;
                            break;
                        case 1:
                            Instantiate(blockWall, new Vector3(floorPos.x, floorPos.y + 0.45f, -0.05f), Quaternion.Euler(0, 0, 90)).transform.parent = _floor.transform;
                            break;
                        case 2:
                            Instantiate(blockWall, new Vector3(floorPos.x - 0.45f, floorPos.y, -0.05f), Quaternion.Euler(0, 0, -180)).transform.parent = _floor.transform;
                            break;
                        case 3:
                            Instantiate(blockWall, new Vector3(floorPos.x, floorPos.y - 0.45f, -0.05f), Quaternion.Euler(0, 0, -90)).transform.parent = _floor.transform;
                            break;
                    }
                }
            }
        }
    }

    public void SetFloorList(GameObject _floor)
    {
        floorList.Add(_floor);
    }


}
