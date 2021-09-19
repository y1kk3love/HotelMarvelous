using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogButton : MonoBehaviour
{
    public int dialogPoint;
    public int dialogIndex;

    public string[] myDialog;

    public void OnClickChoiceButton()
    {
        ScenesManager.Instance.isOnChoice = false;

        ScenesManager.Instance.SetDialogPointInfo(dialogPoint, dialogIndex);

        if (myDialog[0] != "")
        {
            ScenesManager.Instance.isimtalking = true;
            ScenesManager.Instance.curMyDialogArr = myDialog;
            ScenesManager.Instance.MonologueProcess();
        }
        else
        {
            ScenesManager.Instance.DialogProcess();
        }

        GameObject content = GameObject.Find("DialogContent");

        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }
}
