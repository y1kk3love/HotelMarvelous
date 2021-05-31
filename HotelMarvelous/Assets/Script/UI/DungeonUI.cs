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

    private Slider hpbar;
    private Slider staminabar;
    private Slider mentalbar;

    private Image itemCounter;
    private Image dispoitemimage;
    private Image itemimage;

    public Sprite[] dispoimagearr;


    void Start()
    {
        cointext = GameObject.Find("CoinText").GetComponent<Text>();
        keytext = GameObject.Find("KeyText").GetComponent<Text>();
        beantext = GameObject.Find("BeanText").GetComponent<Text>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        hpbar = gameObject.transform.Find("HPBar").GetComponent<Slider>();
        hptext = hpbar.transform.Find("HPText").GetComponent<Text>();
        staminabar = gameObject.transform.Find("StaminaBar").GetComponent<Slider>();
        itemCounter = GameObject.Find("ItemCounter").GetComponent<Image>();
        itemimage = GameObject.Find("ItemImage").GetComponent<Image>();
        dispoitemimage = transform.Find("DispoItem").GetComponent<Image>();
        mentaltext = GameObject.Find("MentalText").GetComponent<Text>();
        mentalbar = GameObject.Find("MentalityBar").GetComponent<Slider>();
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

        hpbar.value = player.GetHp();
        hptext.text = hpbar.maxValue + " / " + Mathf.Ceil(player.GetHp());

        mentalbar.value = player.GetMentality();
        mentaltext.text = string.Format("{0:00.0} ", player.GetMentality());

        cointext.text = " Coin : " + player.GetCoin().ToString();
        keytext.text = " Key : " + player.GetKeys().ToString();
        beantext.text = " Bean : " + player.GetBeans().ToString();

        staminabar.value = player.GetStamina();
        itemCounter.fillAmount = player.GetItemCount();
    }
}
