using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public int openingDir;
    // 1 --> need bottomdoor
    // 2 --> need topdoor
    // 3 --> need leftdoor
    // 4 --> need rightdoor
    int count = 0;
    private int rand;
    public bool spawned = false;
    private RoomTemplates templates;
    

    void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        if(count < 4)
        {
            Invoke("Spawn", 5f);
        }

    }

    void Spawn()
    {
        if (!spawned)
        {
            
            switch (openingDir)
            {
                case 1:
                    rand = Random.Range(0, templates.bottomRooms.Length);
                    Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
                    break;
                case 2:
                    rand = Random.Range(0, templates.topRooms.Length);
                    Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation);
                    break;
                case 3:
                    rand = Random.Range(0, templates.leftRooms.Length);
                    Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation);
                    break;
                case 4:
                    rand = Random.Range(0, templates.rightRooms.Length);
                    Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation);
                    break;
            }
            count++;
            spawned = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("SpawnPoint") && other.GetComponent<MapSpawner>().spawned == true)
        {
            Destroy(this);
        }
    }
}
