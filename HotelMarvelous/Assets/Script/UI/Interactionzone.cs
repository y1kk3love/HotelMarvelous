using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactionzone : MonoBehaviour
{
    public INTERACTION zoneType;
    public DIALOGZONE dialogPoint;
    public int dialogIndex;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if (other.CompareTag("Player") && zoneType == INTERACTION.DUNGEON)
        {
            ScenesManager.Instance.MoveToScene("Dungeon");
        }
    }
}

