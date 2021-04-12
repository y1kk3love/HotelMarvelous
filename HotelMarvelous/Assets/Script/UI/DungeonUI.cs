using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    private Player player;
    private ResourceManager resource;

    private Text hptext;
    private Text mentaltext;

    private Slider hpbar;
    private Slider staminabar;
    private Slider mentalbar;

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
        itemCounter = GameObject.Find("ItemCounter").GetComponent<Image>();
        itemimage = GameObject.Find("ItemImage").GetComponent<Image>();
        dispoitemimage = transform.Find("DispoItem").GetComponent<Image>();
        resource = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
        mentaltext = GameObject.Find("MentalText").GetComponent<Text>();
        mentalbar = GameObject.Find("MentalityBar").GetComponent<Slider>();
    }

    void Update()
    {
        itemimage.sprite = resource.GetItemSprite(player.GetItemCode());

        switch (player.GetDispoItemCode())
        {
            case 0:
                dispoitemimage.sprite = dispoimagearr[(int)ITEMCODE.CROWN];
                break;
            case 1:
                dispoitemimage.sprite = dispoimagearr[(int)ITEMCODE.SLOTMACHINE];
                break;
        }

        hpbar.value = player.GetHp();
        hptext.text = hpbar.maxValue + " / " + player.GetHp();

        mentalbar.value = player.GetMentality();
        mentaltext.text = string.Format("{0:00.0} ", player.GetMentality());

        staminabar.value = player.GetStamina();
        itemCounter.fillAmount = player.GetItemCount();
    }
}
