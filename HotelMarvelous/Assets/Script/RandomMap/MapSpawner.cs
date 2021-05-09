using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OPENDIR
{
    BOTTOM,
    TOP,
    LEFT,
    RIGHT,
    CHECKROOM
}

public class MapSpawner : MonoBehaviour
{
    public OPENDIR opendir;

    private int rand;
    public bool spawned = false;
    private RoomTemplates templates;
    

    void Start()
    {
        templates = GameObject.Find("RoomTamplates").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.5f);

    }

    void Spawn()
    {
        if (!spawned)
        {
            rand = Random.Range(0, templates.bottomRooms.Length);

            switch (opendir)
            {
                case OPENDIR.BOTTOM:
                    Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation);
                    break;
                case OPENDIR.TOP:
                    Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
                    break;
                case OPENDIR.LEFT:
                    Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation);
                    break;
                case OPENDIR.RIGHT:
                    Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation);
                    break;
            }

            Destroy(gameObject);
            //spawned = true;
            Debug.Log("Game Pause");
            Time.timeScale = 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.name == "Destroyer")
        {
            Destroy(gameObject);
        }
        if(other.CompareTag("SpawnPoint"))
        {
            if(other.GetComponent<MapSpawner>().spawned == false && !spawned)
            {
                Instantiate(templates.closedRoom, transform.position, Quaternion.identity);
                spawned = true;
                other.GetComponent<MapSpawner>().spawned = true;
                //Destroy(other.gameObject);
                //Destroy(gameObject);
            }

            spawned = true;
        }
    }
}
