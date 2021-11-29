using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloraFog : MonoBehaviour
{
    private GameObject player;

    private float timer = 1f;

    void Start()
    {
        Destroy(this.gameObject, Random.Range(4f, 6f));
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (player == null)
        {
            player = GameObject.Find("Player");
        }
        else
        {
            transform.position += (player.transform.position - transform.position).normalized;       
        }

        if(Vector3.Distance(transform.position, player.transform.position) <= 3f & timer < 0)
        {
            player.GetComponent<Player>().SetDamage(1f);

            timer = Random.Range(0.5f, 1f);
        }
    }
}
