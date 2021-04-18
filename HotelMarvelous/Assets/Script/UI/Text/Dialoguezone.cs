using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            SceneManager.LoadScene("Dungeon");
        }
    }
}

