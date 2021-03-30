using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackID : Player
{
    

    void Start()
    {
        if(WeaponID == WEAPONID.SWORD)
        {
            
        }
        else if (WeaponID == WEAPONID.SPEAR)
        {
            WeaponID = WEAPONID.SPEAR;
        }
    }

    public GameObject GetAttackRange(int _attnum)
    {
        return transform.GetChild(_attnum).gameObject;
    }
}
