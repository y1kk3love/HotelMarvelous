using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Transform target;
    private float targetY = 2.5f;

    private float xRotMax = 60;
    private float rotSpeed = 500;
    private float scrollSpeed = 500;

    private float distance = 10;
    private float minDistance = 5;
    private float maxDistance = 15;

    private float xRot;
    private float yRot;
    private Vector3 targetPos;
    private Vector3 dir;


    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            xRot += Input.GetAxis("Mouse Y") * rotSpeed * Time.deltaTime;
            yRot += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;
        }

        distance += -Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime;

        xRot = Mathf.Clamp(xRot, -xRotMax, xRotMax);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        targetPos = target.position + Vector3.up * targetY;

        dir = Quaternion.Euler(-xRot, yRot, 0f) * Vector3.forward;
        transform.position = targetPos + dir * -distance;
    }

    void LateUpdate()
    {
        transform.LookAt(targetPos);
    }
}