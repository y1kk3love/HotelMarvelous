using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    private Lobby lobby;

    private int id;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lobby = GameObject.FindGameObjectWithTag("Lobby").GetComponent<Lobby>();
            lobby.CounterEnter(true);
            Debug.Log("들어왔어요~");
        }
    }

    public int CheckID()
    {
        return id;
    }
}

