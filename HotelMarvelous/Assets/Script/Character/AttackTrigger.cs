using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    private int damage;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            other.GetComponent<Monster>().MonGetDamage(damage);
        }
    }

    public void SetDamage(int _damage)
    {
        damage = _damage;
    }
}
