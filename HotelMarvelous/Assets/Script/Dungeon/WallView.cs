using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallView : MonoBehaviour
{
    Animator anim;

    void Update()
    {
        if(anim == null)
        {
            anim = transform.GetComponent<Animator>();
        }

        if(Vector3.Distance(transform.position, Camera.main.transform.position) < 13f)
        {
            anim.SetBool("Show", false);
        }
        else
        {
            anim.SetBool("Show", true);
        }
    }
}
