using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMover : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            string[] doorname = transform.parent.name.Split(' ');

            switch (doorname[0])
            {
                case "Top":
                    Debug.Log(doorname[0]);
                    break;
                case "Right":
                    Debug.Log(doorname[0]);
                    break;
                case "Bottom":
                    Debug.Log(doorname[0]);
                    break;
                case "Left":
                    Debug.Log(doorname[0]);
                    break;
            }
        }
    }
}
