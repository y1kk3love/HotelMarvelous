using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    public int moncount;
    public int RoomType = 0;

    private Vector3 leftPos;
    private Vector3 rightPos;
    private Vector3 rewardPos;

    private bool isplayerhere = false;
    private bool isclear = false;

    private GameObject minimapcam;
    private GameObject rewardprefab;

    private GameObject[] monsterarr;

    void Start()
    {
        rewardprefab = ResourceManager.Instance.LoadGameObject("Prefab/Maps/Reward");
        minimapcam = GameObject.Find("MiniMapCamera");

        leftPos = transform.Find("LeftWayPoint").position;
        rightPos = transform.Find("RightWayPoint").position;

        if(RoomType == 0)
        {
            rewardPos = transform.Find("RewardPos").position;
            transform.Find("HallwayDoor_L").GetComponent<Animation>().Stop();
            transform.Find("HallwayDoor_R").GetComponent<Animation>().Stop();
        }
        else
        {
            transform.Find("LiftDoor").GetComponent<Animation>().Stop();
            transform.Find("MoveLift").GetComponent<Animation>().Stop();
        }
    }

    void Update()
    {
        if (!isclear)
        {
            monsterarr = GameObject.FindGameObjectsWithTag("Monster");

            if (monsterarr.Length == 0 && isplayerhere)
            {
                isclear = true;
                Instantiate(rewardprefab, rewardPos, Quaternion.identity);

                if(RoomType == 0)
                {
                    transform.Find("HallwayDoor_L").GetComponent<Animation>().Play();
                    transform.Find("HallwayDoor_R").GetComponent<Animation>().Play();
                }
                else
                {
                    transform.Find("LiftDoor").GetComponent<Animation>().Play();
                    transform.Find("MoveLift").GetComponent<Animation>().Play();
                }
            }
        }

        if (isplayerhere)
        {
            minimapcam.transform.position = new Vector3(transform.position.x, 15f, transform.position.z);
        }
    }

    public void InstantiateMonster()
    {
        for (int i = 0; i < moncount; i++)
        {
            Debug.Log(leftPos);
            float x = Random.Range(leftPos.x, rightPos.x);
            float z = Random.Range(leftPos.z, rightPos.z);
            int id = Random.Range(0, 2);

            GameObject mon = Instantiate(ResourceManager.Instance.GetMonsterArr(0), new Vector3(x, 0.6f, z), Quaternion.identity);
            Monster monster = mon.GetComponent<Monster>();
            monster.monsterid = id;
        }
    }

    public void SetBoolisplayerhere(bool _bool)
    {
        isplayerhere = _bool;
    }
}
