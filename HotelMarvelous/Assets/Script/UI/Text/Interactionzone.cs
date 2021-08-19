using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactionzone : MonoBehaviour
{
    public INTERACTION zoneType;
    public int dialogPoint;
    public int dialogIndex;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && zoneType == INTERACTION.DUNGEON)
        {
            ScenesManager.Instance.MoveToScene("Dungeon");
        }
    }
}

