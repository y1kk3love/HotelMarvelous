using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMover : MonoBehaviour
{
    public GameObject door;

    public DungeonMaker manager;

    public DIRECTION doorDir;

    public byte x, y;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<Player>().stopAllMove)
        {
            Vector2 pos = new Vector2(x * 18, y * 18);
            Vector2 dir = new Vector2(0, 0);

            switch (doorDir)
            {
                case DIRECTION.TOP:
                    dir = new Vector2(0, 18);                   
                    break;
                case DIRECTION.RIGHT:
                    dir = new Vector2(18, 0);
                    break;
                case DIRECTION.BOTTOM:
                    dir = new Vector2(0, -18);
                    break;
                case DIRECTION.LEFT:
                    dir = new Vector2(-18, 0);
                    break;
            }

            pos += dir;
            manager.MoveNextRoom(pos, dir);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<Player>().stopAllMove)
        {
            Vector2 pos = new Vector2(x * 18, y * 18);
            Vector2 dir = new Vector2(0, 0);

            switch (doorDir)
            {
                case DIRECTION.TOP:
                    dir = new Vector2(0, 18);
                    break;
                case DIRECTION.RIGHT:
                    dir = new Vector2(18, 0);
                    break;
                case DIRECTION.BOTTOM:
                    dir = new Vector2(0, -18);
                    break;
                case DIRECTION.LEFT:
                    dir = new Vector2(-18, 0);
                    break;
            }

            pos += dir;
            manager.MoveNextRoom(pos, dir);
        }
    }
}
