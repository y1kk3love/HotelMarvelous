using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallView : MonoBehaviour
{
    private GameObject Wall;
    private Animator anim;

    void Start()
    {
        Wall = transform.Find("Wall").gameObject;
    }

    void Update()
    {
        if(anim == null)
        {
            anim = transform.GetComponent<Animator>();
        }

        if(Vector3.Distance(transform.position, Camera.main.transform.position) < 13f)
        {
            Wall.SetActive(false);
        }
        else
        {
            Wall.SetActive(true);
        }
    }
}
