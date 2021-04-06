using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSkill : MonoBehaviour
{
    private float skilldelay = 0;

    void Update()
    {
        skilldelay += Time.deltaTime;

        if (skilldelay >= 3)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            StartCoroutine(FireTile(other, 20));
        }
    }

    IEnumerator FireTile(Collider _other, int _damage)
    {
        if(_other != null)
        {
            _other.GetComponent<Monster>().MonGetDamage(_damage);
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(FireTile(_other, _damage));
    }
}
