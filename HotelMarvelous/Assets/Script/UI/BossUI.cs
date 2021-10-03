using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    private Monster boss;

    public Slider hpBar;
    public Text bossname;

    void Start()
    {
        SetBossInfo();
    }

    public void SetBossInfo()
    {
        Monster boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Monster>();

        hpBar.maxValue = boss.GetBossHP();
        hpBar.value = boss.GetBossHP();
        bossname.text = boss.monsterid.ToString();
    }

    void Update()
    {
        if(boss != null)
        {
            hpBar.value = boss.GetBossHP();
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
