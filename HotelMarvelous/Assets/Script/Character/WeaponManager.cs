using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private Player player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if(player.WeaponID == WEAPONID.SWORD)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                GameObject _range = transform.Find("Sword").GetChild(i).gameObject;

                if (_range != null)
                {
                    player.SetAtkRangList(_range);
                    _range.SetActive(false);
                }
            }
        }
    }
}
