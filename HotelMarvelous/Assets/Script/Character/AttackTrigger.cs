using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    private float damage;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            other.GetComponent<Monster>().MonGetDamage(damage);
        }
    }

    public void SetDamage(float _damage)
    {
        damage = _damage;
    }
}
