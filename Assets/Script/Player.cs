using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private float vertical;
    private float horizontal;

    private float verticalraw;
    private float horizontalraw;

    private Vector3 targetrotation;

    private float rotationspeed = 10;
    private float speed = 200f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        horizontalraw = Input.GetAxis("Horizontal");
        verticalraw = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(horizontal, 0, vertical);
        Vector3 inputraw = new Vector3(horizontalraw, 0, verticalraw);

        if(input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }
        if (inputraw.sqrMagnitude > 1f)
        {
            inputraw.Normalize();
        }
        if (inputraw != Vector3.zero)
        {
            targetrotation = Quaternion.LookRotation(input).eulerAngles;
        }

        rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetrotation.x, Mathf.Round(targetrotation.y / 45) * 45, targetrotation.z), Time.deltaTime * rotationspeed);

        Vector3 vel = input * speed * Time.deltaTime;
        rb.velocity = vel;
    }
}
