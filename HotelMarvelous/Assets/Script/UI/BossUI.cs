using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    private Monster boss;

    public Slider hpBar;
    public Text bossname;

    public void SetBossInfo()
    {
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Monster>();

        if(boss != null)
        {
            hpBar.maxValue = boss.GetBossHP();
            hpBar.value = boss.GetBossHP();
            bossname.text = boss.monsterid.ToString();
        }
    }

    void Update()
    {
        if(boss != null)
        {
            SetBossInfo();
        }
        else
        {
            GameObject monster = GameObject.FindGameObjectWithTag("Boss");
            
            if(monster != null)
            {
                boss = monster.GetComponent<Monster>();
            }
        }

    }
}
