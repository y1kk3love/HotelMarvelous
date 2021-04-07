using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    private Player player;
    private ResourceManager resource;

    private Text hptext;

    private Slider hpbar;
    private Slider staminabar;

    private Image itemCounter;
    private Image dispoitemimage;
    private Image itemimage;

    public Sprite[] dispoimagearr;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        hpbar = gameObject.transform.Find("HPBar").GetComponent<Slider>();
        hptext = hpbar.transform.Find("HPText").GetComponent<Text>();
        staminabar = gameObject.transform.Find("StaminaBar").GetComponent<Slider>();
        itemCounter = GameObject.FindGameObjectWithTag("ItemCounter").GetComponent<Image>();
        itemimage = GameObject.FindGameObjectWithTag("ItemCounter").transform.GetChild(0).GetComponent<Image>();
        dispoitemimage = transform.Find("DispoItem").GetComponent<Image>();
        resource = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
    }

    void Update()
    {
        itemimage.sprite = resource.GetItemSprite(player.GetItemCode());

        switch (player.GetDispoItemCode())
        {
            case 0:
                dispoitemimage.sprite = dispoimagearr[0];
                break;
            case 1:
                dispoitemimage.sprite = dispoimagearr[1];
                break;
        }

        hpbar.value = player.GetHp();
        hptext.text = hpbar.maxValue + " / " + player.GetHp();

        staminabar.value = player.GetStamina();
        itemCounter.fillAmount = player.GetItemCount();
    }
}
