using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Animator anim;
    public GameObject attack;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("Attack01");
        }

        if (Input.GetKey(KeyCode.Space))
        {
            attack.SetActive(true);
        }
        else
        {
            attack.SetActive(false);
        }

        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("Run", true);
        }
        else
        {
            anim.SetBool("Run", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }
}
