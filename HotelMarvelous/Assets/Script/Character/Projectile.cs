using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject effect;

    public Vector3 targetPos;

    private float speed = 3;

    void Update()
    {
        transform.position += targetPos * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetComponent<Player>().SetDamage(10);

            Instantiate(effect, transform.position, Quaternion.identity);

            Destroy(transform.gameObject);
        }
        else
        {
            targetPos = new Vector3(0, 0, 0);

            Instantiate(effect, transform.position, Quaternion.identity);

            Destroy(transform.gameObject, 3);
        }
    }
}
