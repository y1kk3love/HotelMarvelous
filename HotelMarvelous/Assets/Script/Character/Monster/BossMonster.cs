﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : MonoBehaviour
{
    public float maxHP = 100;
    public float curHP = 100;

    public void MonGetDamage(float _damage)
    {
        curHP -= _damage;

        if(curHP <= 0)
        {
            Destroy(transform.gameObject, 0.5f);
        }
    }
}
