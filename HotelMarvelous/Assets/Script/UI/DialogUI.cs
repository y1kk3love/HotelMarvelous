using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    public GameObject textBar;
    public GameObject pressimage;
    public Image profile;
    public Text text;

    private string targetDialog;

    private float interval = 1.0f;

    public int dialogSpeed = 20;
    private int curTextIndex = 0;

    void Awake()
    {
        textBar.SetActive(false);
        pressimage.SetActive(false);
    }

    public void SetProfile(Sprite _profile)
    {
        pressimage.SetActive(true);

        profile.sprite = _profile;
    }

    public void SetDialog(string _text)
    {
        DialogAnimProcess(_text);
        textBar.SetActive(true);
    }

    public void DialogFinish()
    {
        GameObject.Find("Player").GetComponent<Player>().isconv = false;

        textBar.SetActive(false);
        pressimage.SetActive(false);
    }

    private void DialogAnimProcess(string _dialog)
    {
        if (ScenesManager.Instance.isDialogAnim)
        {
            text.text = _dialog;

            CancelInvoke();

            ScenesManager.Instance.isDialogAnim = false;
        }
        else
        {
            targetDialog = _dialog;
            DiaLogAnimStart();
        }
    }

    private void DiaLogAnimStart()
    {
        text.text = "";
        curTextIndex = 0;

        interval = 1.0f / dialogSpeed;
        ScenesManager.Instance.isDialogAnim = true;

        Invoke("DialogAimation", interval);
    }

    private void DialogAimation()
    {
        if(text.text == targetDialog)
        {
            ScenesManager.Instance.isDialogAnim = false;
            return;
        }

        text.text += targetDialog[curTextIndex];
        curTextIndex++;

        Invoke("DialogAimation", interval);
    }
}
