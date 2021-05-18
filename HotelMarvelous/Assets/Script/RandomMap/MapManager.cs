using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ROOMTYPE : byte
{
    EMPTY,
    NEEDROOM,
    HALLWAY,
    GUEST,
    NPC
}

public enum ROOMDIR
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT
}

/*
public class MapManager : MonoBehaviour
{
    public GameObject roomprefab;       //임시 나중에 리소스에서 불러오는것 잊지마요!!
    public GameObject floorprefab;

    public ROOMTYPE roomtype;

    public List<GameObject> instRoomList = new List<GameObject>();
    public List<Vector3> needRoomPosList = new List<Vector3>();

    public int[,] mapboard = new int[50, 50];
    public int curRoomCount = 0;
    public int maxRoomCount = 5;

    public string XY;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string[] xyarr = XY.Split(',');

            int _x = int.Parse(xyarr[0]);
            int _y = int.Parse(xyarr[1]);


            Debug.Log(mapboard[_x + 25, _y + 25]);
        }

        if (Input.GetKeyDown(KeyCode.S))
            OnclickMapSpawn();
        if (Input.GetKeyDown(KeyCode.D))
            OnclickMadelete();
    }

    public void OnclickMapSpawn()
    {
        StartCoroutine(InstnatiateRoom(true));
    }

    public void OnclickMadelete()
    {
        foreach (GameObject go in instRoomList)
        {
            Destroy(go);
        }
        needRoomPosList.Clear();
        instRoomList.Clear();
        mapboard = new int[50, 50];
        curRoomCount = 0;
    }

    IEnumerator InstnatiateRoom(bool _start)
    {
        if (_start)
        {
            GameObject room = Instantiate(roomprefab, new Vector3(0, 0, 0), Quaternion.identity);
            room.transform.GetComponent<RoomInfo>().roomType = roomtype;

            curRoomCount++;
            instRoomList.Add(room);
        }
        else
        {
            for(int i = 0; i < needRoomPosList.Count - 1; i++)
            {
                yield return new WaitForSeconds(1f);

                Vector2 v2pos = needRoomPosList[i];
                byte _type = (byte)needRoomPosList[i].z;

                Debug.Log(v2pos.x + " , " + v2pos.y);
                if(mapboard[(byte)(v2pos.x + 25), (byte)(v2pos.y + 25)] != (byte)ROOMTYPE.EMPTY)
                {
                    continue;
                }
                else
                {
                    switch (_type)
                    {
                        case (byte)ROOMTYPE.HALLWAY:
                            GameObject guest = Instantiate(roomprefab, v2pos, Quaternion.identity);
                            guest.transform.GetComponent<RoomInfo>().roomType = ROOMTYPE.HALLWAY;

                            instRoomList.Add(guest);
                            break;
                        case (byte)ROOMTYPE.GUEST:
                            GameObject hallway = Instantiate(roomprefab, v2pos, Quaternion.identity);
                            hallway.transform.GetComponent<RoomInfo>().roomType = ROOMTYPE.GUEST;

                            instRoomList.Add(hallway);
                            break;
                    }
                }
            }

            needRoomPosList.Clear();
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(InstnatiateRoom(false));
    }
}
*/

public class MapManager : MonoBehaviour
{

}
