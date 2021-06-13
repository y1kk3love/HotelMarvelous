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

public enum WEAPONID
{
    SWORD,
    SPEAR
}

public enum ITEMCODE : byte
{
    CROWN,
    SLOTMACHINE
}

public enum RESOULUTION : byte
{
    R1336X768,
    R1440X900,
    R1920X1080,
    R2560X1440,
    R3480X2160
}

public enum FULLSCREENS : byte
{
    WINDOWED,
    FULLSCREEN
}

public enum KEYSETBUTTON : byte
{
    RECHARGE,
    DISPOSABLE,
    RUN,
    MINIMAP,
    PAUSE,
    TREASURE
}