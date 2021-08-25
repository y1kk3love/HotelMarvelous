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

    public Text extraLife;
    public Text coin;
    public Text key;
    public Text bean;

    public Image profile;
    public Image itemcounter;
    public Image dispoitem;


    void Start()
    {
        GameObject prefab = Resources.Load("Prefab/Characters/Player") as GameObject;
        player = Instantiate(prefab, new Vector3(0, 1, 0), Quaternion.identity).GetComponent<Player>();
        player.gameObject.name = "Player";
        stat = player.GetStatus();
    }

    private void Update()
    {
        
    }

    #region ----------------------------[Test]----------------------------

    public void Test_AddItemCount()
    {
        stat.curItemStack++;
    }

    public void Test_AddDisoItem()
    {
        stat.curDispoItemIndex = 1;
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
