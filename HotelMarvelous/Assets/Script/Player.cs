﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private bool isattack = false;

    private float velocity = 5f;
    private float turnspeed = 10f;

    private Vector2 input;
    private float angle;

    private Quaternion targetrotation;
    private Transform cam;

    private Animator anim;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        GetInput();
        Attack();

        if (Mathf.Abs(input.x)< 1 && Mathf.Abs(input.y) < 1)
        {
            anim.SetBool("Run", false);
            return;
        }
        if (isattack)
        {
            return;
        }

        CalculateDirection();
        Rotate();
        Move();
    }

    void GetInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(Attack_01());
        }
    }

    void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += cam.eulerAngles.y;
    }

    void Rotate()
    {
        targetrotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, turnspeed * Time.deltaTime);
    }

    void Move()
    {
        anim.SetBool("Run", true);
        transform.position += transform.forward * velocity * Time.deltaTime;
    }

    IEnumerator Attack_01()
    {
        anim.SetBool("Attack_01", true);
        isattack = true;

        yield return new WaitForSeconds(1.02f);

        anim.SetBool("Attack_01", false);
        isattack = false;
    }
}
