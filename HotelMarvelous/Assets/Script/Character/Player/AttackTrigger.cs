﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            float damage = DataManager.Instance.CalculateDamage();

            other.GetComponent<Monster>().MonGetDamage(damage);
        }
    }
}