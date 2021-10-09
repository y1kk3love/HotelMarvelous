using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus
{
    public int damage = 30;

    public float hp = 30;
    public float maxHp = 30;
    public float criticalPercent = 3.0f;
    public float stamina = 20.0f;
    public float runspeed = 1.5f;
    public float speed = 1f;
    public float mentality = 20.0f;
    public float maxMentality = 20.0f;
    public byte extraLife = 1;
    public int defense = 5;

    public byte coin = 0;
    public byte roomKeys = 0;
    public byte beans = 0;

    public byte curItemIndex = 1;
    public byte curItemMax = 0;
    public byte curItemStack = 1;

    public byte curDispoItemIndex = 1;
}
