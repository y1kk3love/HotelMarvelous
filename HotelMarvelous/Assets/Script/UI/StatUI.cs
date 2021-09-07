using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    private Player player;
    private PlayerStatus stat;

    public Slider hpBar;
    public Slider staminaBar;
    public Slider mentalityBar;

    public Text hpText;
    public Text mentalityText;
    public Text extraLife;
    public Text coin;
    public Text key;
    public Text bean;

    public Image profile;
    public Image itemcounter;
    public Image itemImage;
    public Image dispoitem;

    public GameObject miniMap;

    public Camera miniMapCamera;

    public Texture wideMapTexture;
    public Texture miniMapTexture;

    private bool isWideMap = false;

    void Start()
    {
        GameObject prefab = Resources.Load("Prefab/Characters/Player") as GameObject;
        player = Instantiate(prefab, new Vector3(3, 1, 3), Quaternion.identity).GetComponent<Player>();
        player.gameObject.name = "Player";
        stat = player.GetStatus();

        miniMapCamera = GameObject.Find("MiniMap Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        ChangeMiniMapSize();

        UIUpdate();
    }

    private void UIUpdate()
    {
        //Debug.Log("스텟 변함");

        stat = player.GetStatus();

        hpBar.maxValue = stat.maxHp;
        hpBar.value = stat.hp;
        hpText.text = string.Format("{0} / {1}", stat.hp, stat.maxHp);

        staminaBar.value = stat.stamina;

        mentalityBar.maxValue = stat.maxMentality;
        mentalityBar.value = stat.mentality;
        mentalityText.text = stat.mentality.ToString("0.0");

        extraLife.text = "X " + stat.extraLife;
        coin.text = "Coin : " + stat.coin;
        key.text = "Key : " + stat.roomKeys;
        bean.text = "Bean : " + stat.beans;

        itemcounter.fillAmount = (float)stat.curItemStack / (float)stat.curItemMax;

        ItemResource _resource = ResourceManager.Instance.GetItemResource(stat.curItemIndex);
        itemImage.sprite = _resource.sprite;

        if (stat.curDispoItemIndex != 255)
        {
            dispoitem.sprite = ResourceManager.Instance.GetDispoItemResource(stat.curDispoItemIndex);
            dispoitem.color = new Vector4(1, 1, 1, 1);
        }
        else
        {
            dispoitem.sprite = null;
            dispoitem.color = new Vector4(1,1,1,0);
        }
        
    }

    #region [MiniMapControl]
    
    private void ChangeMiniMapSize()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            RectTransform minimap = miniMap.GetComponent<RectTransform>();
            RawImage maptexture = miniMap.GetComponent<RawImage>();

            if (isWideMap)
            {
                minimap.transform.position = new Vector3(960, 540, 0);

                minimap.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1800);
                minimap.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1000);

                miniMapCamera.targetTexture = wideMapTexture as RenderTexture;
                maptexture.texture = wideMapTexture;

                isWideMap = false;
            }
            else
            {
                minimap.transform.position = new Vector3(1760, 920, 0);

                minimap.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 280);
                minimap.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 280);

                miniMapCamera.targetTexture = miniMapTexture as RenderTexture;
                maptexture.texture = miniMapTexture;

                isWideMap = true;
            }
        }
    }

    #endregion

    #region ----------------------------[Test]----------------------------

    public void Test_AddItemCount()
    {
        player.Test_ItemCounerAdd();
    }

    public void Test_AddDisoItem()
    {
        player.Test_ChangeDispoItem(1);
    }

    public void Test_GamePause()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    #endregion
}
