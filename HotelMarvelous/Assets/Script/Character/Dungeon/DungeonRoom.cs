using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    public int moncount;
    private Vector3 leftPos;
    private Vector3 rightPos;

    private bool isplayerhere = false;
    private bool isclear = false;

    private GameObject minimapcam;
    private GameObject rewardPos;
    public GameObject monsteprefab;
    public GameObject rewardprefab;

    private GameObject[] monsterarr;

    void Start()
    {
        rewardPos = transform.Find("RewardPos").gameObject;
        minimapcam = GameObject.Find("MiniMapCamera");

        leftPos = transform.Find("LeftWayPoint").position;
        rightPos = transform.Find("RightWayPoint").position;
    }

    void Update()
    {
        if (!isclear)
        {
            monsterarr = GameObject.FindGameObjectsWithTag("Monster");

            if (monsterarr.Length == 0 && isplayerhere)
            {
                isclear = true;
                Instantiate(rewardprefab, rewardPos.transform.position, transform.rotation);
                transform.Find("LeftDoor").gameObject.SetActive(false);
                transform.Find("RightDoor").gameObject.SetActive(false);
            }
        }

        if (isplayerhere)
        {
            minimapcam.transform.position = new Vector3(transform.position.x, 50f, transform.position.z);
        }
    }

    public void InstantiateMonster()
    {
        for (int i = 0; i < moncount; i++)
        {
            float x = Random.Range(leftPos.x, rightPos.x);
            float z = Random.Range(leftPos.z, rightPos.z);
            int id = Random.Range(0, 2);

            GameObject mon = Instantiate(monsteprefab, new Vector3(x, 0.6f, z), Quaternion.identity);
            Monster monster = mon.GetComponent<Monster>();
            monster.monsterid = id;
        }
    }

    public void SetBoolisplayerhere(bool _bool)
    {
        isplayerhere = _bool;
    }
}
