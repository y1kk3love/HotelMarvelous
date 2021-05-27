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

public enum DIRECTION : byte
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT
}

public enum WALLSTATE : byte
{
    BLOCK,
    EMPTY,
    DOOR
}

public class MapManager : MonoBehaviour
{
    public ROOMTYPE roomType;

    public FloorInfo[,] RoomBoard = new FloorInfo[50, 50];

    public GameObject roomPrefab;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;

    public byte maxRoomCount = 5;
    public byte curRoomCount = 0;
    public byte curRoomIndex = 0;

    public void OnclickStartButton()
    {
        GameObject room = Instantiate(roomPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        RoomController roomcon = room.transform.GetComponent<RoomController>();
        roomcon.roomType = roomType;

        if(roomType == ROOMTYPE.HALLWAY)
        {
            roomcon.enterDir = (DIRECTION)Random.Range(0,4);
        }
    }

    public void OnclickResetButton()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Rooms");

        foreach(GameObject _room in rooms)
        {
            Destroy(_room);
        }

        RoomBoard = new FloorInfo[50, 50];
        curRoomCount = 0;
    }

    public void OnclickBuildWall()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Rooms");

        foreach (GameObject _room in rooms)
        {
            _room.transform.GetComponent<RoomController>();
        }
    }
}

public class FloorInfo
{
    public byte roomIndex = 0;
    public GameObject floorObject = null;
    public GameObject[] wallObjArr = new GameObject[4];
    public ROOMTYPE roomType = ROOMTYPE.EMPTY;
}