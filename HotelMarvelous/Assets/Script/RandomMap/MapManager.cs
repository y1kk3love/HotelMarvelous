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

public enum DIRECTION
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT
}

public enum WALLSTATE
{
    BLOCK,
    EMPTY,
    DOOR
}

public class MapManager : MonoBehaviour
{
    public FloorInfo[,] RoomBoard = new FloorInfo[50, 50];

    public GameObject roomPrefab;
    public GameObject floorPrefab;

    public byte curRoomCount = 0;

    private void OnclickStartButton()
    {
        Instantiate(roomPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void OnclickResetButton()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Rooms");

        foreach(GameObject _room in rooms)
        {
            Destroy(_room);
        }

        RoomBoard = new FloorInfo[50, 50]; ;
    }
}

public class FloorInfo
{
    public byte roomIndex = 0;
    public Vector2 floorPos = new Vector3(0,0);
    public GameObject floorObject = null;
    public GameObject[] wallObj = new GameObject[4];
    public ROOMTYPE roomType = ROOMTYPE.EMPTY;
}