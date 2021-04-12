using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dialoguezone : MonoBehaviour
{
    public int id;

    public int? CheckID()
    {
        if(id != 100)
        {
            return id;
        }
        else
        {
            return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && id == 100)
        {
            SceneManager.LoadScene("Dungeon");
        }
    }
}

