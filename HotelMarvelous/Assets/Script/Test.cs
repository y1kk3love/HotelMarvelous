using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject[] objarr;

    public Animator anim;

    void Start()
    {
        ResourceManager.Instance.LoadResources();

        for (int i  = 0; i < objarr.Length; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));

            Instantiate(objarr[i], pos, Quaternion.identity);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            anim.SetTrigger("Hit");
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            anim.SetBool("Idle", true);
        }
        else
        {
            anim.SetBool("Idle", false);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            anim.SetBool("Down", true);
        }
        else
        {
            anim.SetBool("Down", false);
        }
    }
}
