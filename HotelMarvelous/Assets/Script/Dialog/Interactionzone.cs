using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactionzone : MonoBehaviour
{
    public INTERACTION portalType;
    public NPCID dialogPoint;
    public int dialogIndex;
    public bool isToNextStage = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if (other.CompareTag("Player") && portalType != INTERACTION.NONE)
        {
            GameObject dungeon = GameObject.Find("DungeonManager");

            if(dungeon != null)
            {
                if (dungeon.GetComponent<DungeonMaker>().IsBossClear())
                {
                    GameObject ui = Resources.Load("Prefab/UI/AskToMoveUI") as GameObject;
                    GameObject obj = Instantiate(ui);
                    AskToMoveUI ask = obj.transform.GetComponent<AskToMoveUI>();
                    ask.scene = portalType;
                    ask.isNextStage = isToNextStage;

                    other.GetComponent<Player>().isconv = true;
                }
            }
        }
    }
}