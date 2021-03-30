using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackID : MonoBehaviour
{
    Player player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if(player.WeaponID == WEAPONID.SWORD)
        {
            
        }
        else if (player.WeaponID == WEAPONID.SPEAR)
        {
            player.WeaponID = WEAPONID.SPEAR;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            Monster monster = other.gameObject.GetComponent<Monster>();
            monster.MonGetDamage(10);
        }
    }

    public GameObject GetAttackRange(int _attnum)
    {
        return transform.GetChild(_attnum).gameObject;
    }
}
