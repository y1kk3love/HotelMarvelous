using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    public int moncount;
    private float xPos;
    private float zPos;

    private bool isclear = false;

    public GameObject monsteprefab;
    public GameObject rewardprefab;

    private GameObject[] monsterarr;

    void Start()
    {
        xPos = transform.Find("LeftWayPoint").position.x;
        zPos = transform.Find("RightWayPoint").position.z;

        for(int i = 0; i < moncount; i++)
        {
            float x = Random.Range(xPos, -xPos);
            float z = Random.Range(zPos, -zPos);
            int id = Random.Range(0, 2);

            GameObject mon = Instantiate(monsteprefab, new Vector3(x, 0.6f, z), Quaternion.identity);
            Monster monster = mon.GetComponent<Monster>();
            monster.monsterid = id;
        } 
    }

    void Update()
    {
        if (!isclear)
        {
            monsterarr = GameObject.FindGameObjectsWithTag("Monster");

            if (monsterarr.Length == 0)
            {
                isclear = true;
                Instantiate(rewardprefab, new Vector3(0, 5.69f, 8.23f), Quaternion.identity);
                transform.Find("LeftDoor").gameObject.SetActive(false);
                transform.Find("RightDoor").gameObject.SetActive(false);
            }
        }
    }
}
