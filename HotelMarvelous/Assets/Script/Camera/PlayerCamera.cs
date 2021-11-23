using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private GameObject player;

    private Vector3 vOffset;

    [SerializeField]
    private float cameraspeed = 3.0f;
    [SerializeField]
    private float camerahight = 20f;

    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            vOffset = transform.position - player.transform.position;       //카메라와 플레이어 사이의 거리 Offset
        }

        Vector3 targetpos = player.transform.position + vOffset;

        transform.position = Vector3.Lerp(transform.position, new Vector3(targetpos.x, camerahight, targetpos.z), Time.deltaTime * cameraspeed);
    }
}
