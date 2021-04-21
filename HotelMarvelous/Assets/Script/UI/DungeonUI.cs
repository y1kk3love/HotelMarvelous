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

    private Slider hpbar;
    private Slider staminabar;
    private Slider mentalbar;

    private Image itemCounter;
    private Image dispoitemimage;
    private Image itemimage;

    public Sprite[] dispoimagearr;



    public bool WEDIDIT = false;
    public Color THX;
    public GameObject obob;



    void Start()
    {
        cointext = GameObject.Find("CoinText").GetComponent<Text>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        hpbar = gameObject.transform.Find("HPBar").GetComponent<Slider>();
        hptext = hpbar.transform.Find("HPText").GetComponent<Text>();
        staminabar = gameObject.transform.Find("StaminaBar").GetComponent<Slider>();
        itemCounter = GameObject.Find("ItemCounter").GetComponent<Image>();
        itemimage = GameObject.Find("ItemImage").GetComponent<Image>();
        dispoitemimage = transform.Find("DispoItem").GetComponent<Image>();
        mentaltext = GameObject.Find("MentalText").GetComponent<Text>();
        mentalbar = GameObject.Find("MentalityBar").GetComponent<Slider>();

        obob = GameObject.Find("깊은감사");
        obob.SetActive(false);
    }

    void Update()
    {
        if (WEDIDIT)
        {
            obob.SetActive(true);
            THX.a += (Time.deltaTime * 0.3f);

            GameObject.Find("깊은감사").GetComponent<Image>().color = THX;
        }



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
        hptext.text = hpbar.maxValue + " / " + player.GetHp();

        mentalbar.value = player.GetMentality();
        mentaltext.text = string.Format("{0:00.0} ", player.GetMentality());

        cointext.text = " Coin : " + player.GetCoin().ToString();

        staminabar.value = player.GetStamina();
        itemCounter.fillAmount = player.GetItemCount();
    }
}
