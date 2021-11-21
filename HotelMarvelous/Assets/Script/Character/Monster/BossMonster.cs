using System.Collections;
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
            GameObject manager = GameObject.Find("DungeonManager");
            manager.GetComponent<DungeonEvent>().isBossDead = true;
            manager.GetComponent<DungeonMaker>().MonsterDead();
            transform.GetComponent<Animation>().Play("sj001_die");

            Destroy(transform.gameObject, 1f);
        }
    }
}
