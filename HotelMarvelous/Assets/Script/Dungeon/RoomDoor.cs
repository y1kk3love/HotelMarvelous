using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STAT
{
    ENTER,
    EXIT,
    Lift
}

public class RoomDoor : MonoBehaviour
{
    public STAT stat;

    private bool isused = false;
    private DungeonRoom dungeon;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isused)
        {
            if (stat == STAT.ENTER)
            {
                isused = true;
                dungeon = transform.parent.GetComponent<DungeonRoom>();
                Debug.Log("생성");
                dungeon.SetBoolisplayerhere(true);
                dungeon.InstantiateMonster();
            }
            else if(stat == STAT.EXIT)
            {
                dungeon = transform.parent.GetComponent<DungeonRoom>();
                dungeon.SetBoolisplayerhere(false);
                other.transform.position = new Vector3(0, 0.05f, 50);
            }
            else
            {

            }
        }
    }
}
