using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEvent : MonoBehaviour
{
    public GameObject papi;
    private GameObject obpapi;
    private GameObject player;

    private Vector3 targetPos;

    public bool isBossDead = false;
    private bool isfaceplayer = false;

    private float timer = 0;

    void Update()
    {
        SpawnNPCPapi();

        MoveToElevator();
    }

    private void SpawnNPCPapi()
    {
        timer += Time.deltaTime;

        if (isBossDead && timer >= 1.5f)
        {
            isBossDead = false;

            GameObject ui = GameObject.Find("BossUI");
            ui.GetComponent<Animator>().SetBool("FadeIn", true);

            GameObject door = GameObject.FindGameObjectWithTag("Door");
            Vector3 doorpos = new Vector3(door.transform.position.x, 0.5f, door.transform.position.z);

            obpapi = Instantiate(papi, doorpos, Quaternion.identity);
            obpapi.transform.forward = new Vector3(1, 0, 0);

            targetPos = GameObject.Find("NPCPoint").transform.position;
        }
    }

    private void MoveToElevator()
    {
        if(obpapi != null)
        {
            if (Vector3.Distance(targetPos, obpapi.transform.position) > 0.5f)
            {
                Vector3 dir = (targetPos - obpapi.transform.position).normalized;
                dir.y = 0;

                obpapi.transform.position += dir * Time.deltaTime * 3;
                obpapi.transform.forward = dir;
            }
            else
            {
                if (player == null)
                {
                    player = GameObject.FindGameObjectWithTag("Player");
                }
                else
                {
                    Vector3 dir = (player.transform.position - obpapi.transform.position).normalized;

                    obpapi.transform.rotation = Quaternion.Lerp(obpapi.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 3);
                }
            }
        }
    }
}
