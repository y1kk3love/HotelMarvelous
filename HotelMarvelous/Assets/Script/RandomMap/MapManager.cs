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
    BOTTOM,
    LEFT
}

public class MapManager : MonoBehaviour
{
    public GameObject roomprefab;       //임시 나중에 리소스에서 불러오는것 잊지마요!!
    public GameObject floorprefab;

    public ROOMTYPE roomtype;

    public List<GameObject> instRoomList = new List<GameObject>();

    public int[,] mapboard = new int[256, 256];
    public int curRoomCount = 0;
    public int maxRoomCount = 5;

    public int[,] GetMapBoard()
    {
        return mapboard;
    }

    public void OnclickMapSpawn()
    {
        GameObject room = Instantiate(roomprefab, new Vector3(0, 0, 0), Quaternion.identity);
        room.transform.GetComponent<RoomInfo>().roomType = roomtype;

        curRoomCount++;
        instRoomList.Add(room);
    }

    public void OnclickMadelete()
    {
        foreach (GameObject go in instRoomList)
        {
            Destroy(go);
        }

        instRoomList.Clear();
        mapboard = new int[256, 256];
        curRoomCount = 0;
    }
}
