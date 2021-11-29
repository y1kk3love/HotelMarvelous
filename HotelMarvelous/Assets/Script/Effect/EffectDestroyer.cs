using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    public float timer = 1;

    void Start()
    {
        Destroy(this.gameObject, timer);
    }
}
