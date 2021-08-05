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

            string tilename = transform.parent.parent.parent.gameObject.name;
            string[] name = tilename.Split('/');
            string[] posname = name[1].Split(',');

            int x = int.Parse(posname[0]) * 18;
            int y = int.Parse(posname[1]) * 18;
            Vector2 pos;

            DungeonMaker manager = GameObject.Find("DungeonMaker").GetComponent<DungeonMaker>();

            switch (doorname[0])
            {
                case "Top":
                    Debug.Log(doorname[0]);

                    pos = new Vector2(x, y + 18);
                    manager.MoveNextRoom(pos);
                    break;
                case "Right":
                    Debug.Log(doorname[0]);

                    pos = new Vector2(x + 18, y);
                    manager.MoveNextRoom(pos);
                    break;
                case "Bottom":
                    Debug.Log(doorname[0]);

                    pos = new Vector2(x, y - 18);
                    manager.MoveNextRoom(pos);
                    break;
                case "Left":
                    Debug.Log(doorname[0]);

                    pos = new Vector2(x - 18, y);
                    manager.MoveNextRoom(pos);
                    break;
            }
        }
    }
}
