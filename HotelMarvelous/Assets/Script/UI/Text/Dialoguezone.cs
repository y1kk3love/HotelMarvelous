using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TEXTZONE : byte
{    
    COUNTER,
    TODUNGEON = 200
}

public class Dialoguezone : MonoBehaviour
{
    public TEXTZONE textzone;
    public byte Dialogueid;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && textzone == TEXTZONE.TODUNGEON)
        {
            ScenesManager.Instance.MoveToScene("Dungeon");
        }
    }
}

