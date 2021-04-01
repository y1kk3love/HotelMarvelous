using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    private Player player;

    private Text hptext;
    private Text staminatext;

    private Slider hpbar;
    private Slider staminabar;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        hpbar = gameObject.transform.Find("HPBar").GetComponent<Slider>();
        hptext = hpbar.transform.Find("HPText").GetComponent<Text>();
        staminabar = gameObject.transform.Find("StaminaBar").GetComponent<Slider>();
        staminatext = staminabar.transform.Find("StaminaText").GetComponent<Text>();
    }

    void Update()
    {
        hpbar.value = player.CheckHp();
        hptext.text = "HP " + hpbar.maxValue + " / " + player.CheckHp();

        staminabar.value = player.CheckStamina();
        staminatext.text = "Stamina" + staminabar.maxValue + " / " + Mathf.FloorToInt(player.CheckStamina());
    }
}
