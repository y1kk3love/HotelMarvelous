[System.Serializable]
public enum ROOMTYPE
{
    EMPTY,
    HALLWAY,
    GUEST,
    NPC,
    BOSS
}

public enum INTERACTION 
{
    NONE,
    LOBBY,
    DUNGEON,
    MENU,
    TITLE
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

public enum DIALOGDATA
{
    POINTINDEX,
    POINTNAME,
    TEXTINDEX,
    TEXTEVENT,
    TEXT
}

public enum DIALOGEVENTDATA
{ 
    EVENTINDEX,
    EVENTMOVETO,
    EVENTREWARD,
    EVENTCHICE,
    EVENTDIALOG
}

public enum NPCID
{
    Keneth,
    Elizabeth
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
    INK
}

public enum DISPOITEM
{
    COIN,
    KEY,
    BEAN,
    MASTERKEY,
    HP,
    MENTAL,
    HEALALL
}

public enum CONSUMITEM : byte
{
    CROWN = 1,
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