using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    private Player player;

    private Text hptext;
    private Slider hpbar;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        hpbar = GameObject.FindGameObjectWithTag("HP").GetComponent<Slider>();
        hptext = GameObject.FindGameObjectWithTag("HP").transform.Find("HPText").GetComponent<Text>();
    }

    void Update()
    {
        hpbar.value = player.CheckHp();
        hptext.text = "HP " + hpbar.maxValue + " / " + player.CheckHp();
    }
}
