using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    private BossMonster boss;

    public Slider hpBar;
    public Text bossname;
    public Image yellowbar;

    private float curBarPer = 1;

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
                
                boss = monster.GetComponent<BossMonster>();
            }
        }

        if(yellowbar.fillAmount >= curBarPer)
        {
            yellowbar.fillAmount -= Time.deltaTime;
        }
    }

    public void SetBossInfo()
    {
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossMonster>();

        if (boss != null)
        {
            hpBar.maxValue = boss.maxHP;
            hpBar.value = boss.curHP;
            bossname.text = "저스티스";

            Invoke("FollowRedBar", 0.5f);
        }
    }

    private void FollowRedBar()
    {
        curBarPer = boss.curHP / boss.maxHP;
    }
}
