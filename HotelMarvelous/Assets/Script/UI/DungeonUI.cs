using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    private Player player;

    private Text hptext;
    private Text mentaltext;
    private Text cointext;
    private Text keytext;
    private Text beantext;
    private Text extraLifeText;

    private Slider hpbar;
    private Slider staminabar;
    private Slider mentalbar;

    private Image itemCounter;
    private Image dispoitemimage;
    private Image itemimage;

    public Sprite[] dispoimagearr;


    void Start()
    {
        cointext = GameObject.Find("Coin_Text").GetComponent<Text>();
        keytext = GameObject.Find("Key_Text").GetComponent<Text>();
        beantext = GameObject.Find("Bean_Text").GetComponent<Text>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        hpbar = GameObject.Find("HPBar_Slider").GetComponent<Slider>();
        hptext = GameObject.Find("HP_Text").GetComponent<Text>();

        staminabar = GameObject.Find("StaminaBar_Slider").GetComponent<Slider>();

        extraLifeText = GameObject.Find("ExtraLife_Text").GetComponent<Text>();

        mentaltext = GameObject.Find("Mental_Text").GetComponent<Text>();
        mentalbar = GameObject.Find("MentalityBar_Slider").GetComponent<Slider>();

        itemCounter = GameObject.Find("ItemCounter_Image").GetComponent<Image>();
        itemimage = GameObject.Find("Item_Image").GetComponent<Image>();

        dispoitemimage = transform.Find("DispoItem_Image").GetComponent<Image>();

        ScenesManager.Instance.ShowPauseButton();
    }

    void Update()
    {
        itemimage.sprite = ResourceManager.Instance.GetItemSprite(player.GetItemCode());

        switch (player.GetDispoItemCode())
        {
            case 0:
                dispoitemimage.sprite = dispoimagearr[(int)ITEMCODE.CROWN];
                break;
            case 1:
                dispoitemimage.sprite = dispoimagearr[(int)ITEMCODE.SLOTMACHINE];
                break;
        }

        hpbar.maxValue = player.GetMaxHp();
        hpbar.value = player.GetHp();
        hptext.text = hpbar.maxValue + " / " + Mathf.Ceil(player.GetHp());

        mentalbar.maxValue = player.GetMaxMentality();
        mentalbar.value = player.GetMentality();
        mentaltext.text = string.Format("{0:00.0} ", player.GetMentality());

        cointext.text = " Coin : " + player.GetCoin().ToString();
        keytext.text = " Key : " + player.GetKeys().ToString();
        beantext.text = " Bean : " + player.GetBeans().ToString();

        staminabar.value = player.GetStamina();
        itemCounter.fillAmount = player.GetItemCount();
    }
}
