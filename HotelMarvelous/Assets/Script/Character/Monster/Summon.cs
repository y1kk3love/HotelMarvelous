using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : MonoBehaviour
{
    public GameObject effect;

    public void StartEffect()
    {
        effect.SetActive(true);

        GameObject player = GameObject.Find("Player");

        if(player != null)
        {
            if(Vector3.Distance(player.transform.position, effect.transform.position) <= 3f)
            {
                player.GetComponent<Player>().SetDamage(5f);
            }
        }

        Destroy(this.gameObject, 2f);
    }
}
