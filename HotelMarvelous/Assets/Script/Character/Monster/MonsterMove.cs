using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    private Animator anim;

    private GameObject obPlayer;

    private Vector3 targetPos;

    private float speed;    

    void Start()
    {
        anim = transform.GetComponent<Animator>();
        obPlayer = GameObject.FindGameObjectWithTag("Player");
        targetPos = transform.position;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 dir;

        float targetDis = Vector3.Distance(transform.position, targetPos); 

        if(targetDis > 5f)
        {
            dir = (targetPos - transform.position).normalized;
        }
        else if(targetDis > 1f)
        {
            dir = (obPlayer.transform.position - transform.position).normalized;
        }
        else
        {
            dir = new Vector3(0, 0, 0);
        }

        transform.position += Time.deltaTime * dir * speed;
    }

    public void SetTargetPos(Vector3 _pos)
    {
        targetPos = _pos;
    }
}
