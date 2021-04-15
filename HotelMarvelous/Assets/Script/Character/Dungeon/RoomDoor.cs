using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STAT
{
    ENTER,
    EXIT
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
            else
            {
                dungeon = transform.parent.GetComponent<DungeonRoom>();
                dungeon.SetBoolisplayerhere(false);
                other.transform.position = new Vector3(3, 0.6f, 80);
            }
        }
    }
}
