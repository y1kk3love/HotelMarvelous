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
        if (other.CompareTag("Player"))
        {
            Vector2 pos = new Vector2(x * 18, y * 18);

            switch (doorDir)
            {
                case DIRECTION.TOP:

                    pos += new Vector2(0, 18);
                    
                    break;
                case DIRECTION.RIGHT:

                    pos += new Vector2(18, 0);
                    manager.MoveNextRoom(pos);
                    break;
                case DIRECTION.BOTTOM:

                    pos += new Vector2(0, -18);
                    manager.MoveNextRoom(pos);
                    break;
                case DIRECTION.LEFT:

                    pos += new Vector2(-18, 0);
                    manager.MoveNextRoom(pos);
                    break;
            }
        }
    }
}
