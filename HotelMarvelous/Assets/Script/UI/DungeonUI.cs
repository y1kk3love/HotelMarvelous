using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    private Player player;

    private Text hptext;

    private Slider hpbar;
    private Slider staminabar;

    private Image itemCounter;
    private Image dispoitemimage;

    public Sprite[] dispoimagearr;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        hpbar = gameObject.transform.Find("HPBar").GetComponent<Slider>();
        hptext = hpbar.transform.Find("HPText").GetComponent<Text>();
        staminabar = gameObject.transform.Find("StaminaBar").GetComponent<Slider>();
        itemCounter = GameObject.FindGameObjectWithTag("ItemCounter").GetComponent<Image>();
        dispoitemimage = transform.Find("DispoItem").GetComponent<Image>();
    }

    void Update()
    {
        switch (player.CheckItemCode())
        {
            case 0:
                dispoitemimage.sprite = dispoimagearr[0];
                break;
            case 1:
                dispoitemimage.sprite = dispoimagearr[1];
                break;
        }

        hpbar.value = player.CheckHp();
        hptext.text = hpbar.maxValue + " / " + player.CheckHp();

        staminabar.value = player.CheckStamina();
        itemCounter.fillAmount = player.CheckItemCount();
    }
}
