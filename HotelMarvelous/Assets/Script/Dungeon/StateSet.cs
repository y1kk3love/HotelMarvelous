[System.Serializable]
public enum ROOMTYPE
{
    EMPTY,
    HALLWAY,
    GUEST,
    NPC
}

public enum INTERACTION 
{
    NONE,
    LOBBY,
    DUNGEON
}


public enum DIALOGDATA
{
    POINTNAME,
    POINTINDEX,
    TEXTINDEX,
    TEXT
}

public enum DIALOGZONE
{
    COUNTER,
    ELEVATOR
}

public enum DIRECTION : byte
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT
}

[System.Serializable]
public enum ROTATION
{
    NORMAL,
    PLUS90,
    PLUS180,
    PLUS270
}

[System.Serializable]
public enum WALLSTATE : byte
{
    BLOCK,
    EMPTY,
    DOOR
}

public enum DROPITEM
{
    COIN,
    KEYS,
    MASTERKEY,
    BEANS,
    HPS,
    HPM,
    HPL,
    MENTALS,
    MENTALM,
    MENTALL,
    TOTALHEALS,
    TOTALHEALM,
    TOTALHEALL
}

public enum MONSTERTYPE
{
    NONE,
    SUPERVIA,
    AVARITIA,
    PORTRAIT,
    INK
}

public enum ITEMCODE : byte
{
    CROWN,
    SLOTMACHINE
}

public enum RESOULUTION : byte
{
    R1336X0768,
    R1440X0900,
    R1920X1080,
    R2560X1440,
    R3480X2160
}

public enum FULLSCREENS : byte
{
    WINDOWED,
    FULLSCREEN
}

public enum KEYSETBUTTON
{
    RECHARGE,
    DISPOSABLE,
    RUN,
    MINIMAP,
    PAUSE,
    TREASURE
}

public enum TOOLEDITUI
{
    TILEMODE,
    MONSTERMODE,
    FURNITUREMODE
}

public enum OBJECTDATA : int
{
    INDEX,
    FURNITURENAME,
    ITEMNAME = 3,
    MAXSTACK,
    MONSTERNAME = 6,
    DISPOITEM = 8,
    DISPOINDEX
}