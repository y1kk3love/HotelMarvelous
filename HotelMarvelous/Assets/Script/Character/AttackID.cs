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
            for(int i = 0; i < transform.childCount; i++)
            {
                GameObject _range = transform.Find("Sword").GetChild(i).gameObject;

                if (_range != null)
                {
                    player.GetAtkRangList(_range);
                    _range.SetActive(false);
                }
            }
        }
        else if (player.WeaponID == WEAPONID.SPEAR)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject _range = gameObject.transform.Find("Sword").GetChild(i).gameObject;

                if (_range != null)
                {
                    player.GetAtkRangList(_range);
                    _range.SetActive(false);
                }
            }
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
