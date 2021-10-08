using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject[] objarr;

    void Start()
    {
        for(int i  = 0; i < objarr.Length; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));

            Instantiate(objarr[i], pos, Quaternion.identity);
        }
    }
    void Update()
    {
        
    }
}
