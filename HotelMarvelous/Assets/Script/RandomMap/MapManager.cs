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

public class MapManager : MonoBehaviour
{
    public FloorInfo[,] RoomBoard = new FloorInfo[50, 50];

    public GameObject roomPrefab;
    public GameObject floorPrefab;

    public byte curRoomCount = 0;

    private void Start()
    {

    }
}

public class FloorInfo
{
    public byte roomIndex = 0;
    public Vector2 floorPos = new Vector3(0,0);
    public GameObject floorObject = null;
    public ROOMTYPE roomType = ROOMTYPE.EMPTY;
}
