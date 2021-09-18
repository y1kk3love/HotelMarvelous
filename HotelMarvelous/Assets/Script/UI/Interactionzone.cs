using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactionzone : MonoBehaviour
{
    public INTERACTION portalType;
    public DIALOGZONE dialogPoint;
    public int dialogIndex;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if (other.CompareTag("Player") && portalType != INTERACTION.NONE)
        {
            string scenes = portalType.ToString();

            ScenesManager.Instance.MoveToScene(scenes);
        }
    }
}