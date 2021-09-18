using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogButton : MonoBehaviour
{
    public int dialogPoint;
    public int dialogIndex;

    public void OnClickChoiceButton()
    {
        ScenesManager.Instance.isOnChoice = false;

        ScenesManager.Instance.DialogProcess((DIALOGZONE)dialogPoint, dialogIndex);

        Player player = GameObject.Find("Player").GetComponent<Player>();
        player.SetNextDialog(dialogPoint, dialogIndex);

        GameObject content = GameObject.Find("DialogContent");

        for(int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }
}
