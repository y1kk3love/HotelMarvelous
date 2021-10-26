using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public GameObject circle;

    void FixedUpdate()
    {
        if (circle.transform.localScale.x < 35f)
        {
            circle.transform.localScale += new Vector3(1, 1, 0);
        }
        else
        {
            if(ScenesManager.Instance.CheckScene() == "Title")
            {
                ScenesManager.Instance.MoveToScene(INTERACTION.MENU);
            }
            else
            {
                ScenesManager.Instance.MoveToScene(INTERACTION.LOBBY);
            }
            
        }
    }
}
