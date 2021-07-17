using System.Collections.Generic;
using UnityEngine;

public class TileInfo
{
    public Vector3 position;

    public GameObject obTile;

    public byte roomIndex;

    public ROOMTYPE roomType = ROOMTYPE.EMPTY;

    public WALLSTATE[] doorArr = new WALLSTATE[4];

    public Dictionary<Vector2, MONSTERTYPE> monSpawnInfoDic = new Dictionary<Vector2, MONSTERTYPE>();
    public Dictionary<Vector2, FurnitureInfo> FurnitureInfoDic = new Dictionary<Vector2, FurnitureInfo>();
}

public class FurnitureInfo
{
    public string name;

    public Vector2 pos;

    public ROTATION dir;
}

[System.Serializable]
public class TileData
{
    public string pos;

    public byte roomIndex;

    public ROOMTYPE roomType = ROOMTYPE.EMPTY;

    public WALLSTATE[] doorArr = new WALLSTATE[4];

    public Dictionary<string, MONSTERTYPE> monSpawnInfoDic = new Dictionary<string, MONSTERTYPE>();
    public Dictionary<string, FurnitureData> FurnitureInfoDic = new Dictionary<string, FurnitureData>();
}

[System.Serializable]
public class FurnitureData
{
    public string name;
    public string pos;

    public ROTATION dir;
}
