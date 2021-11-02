using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    public GameObject textBar;
    public GameObject pressimage;
    public GameObject buttonContent;
    public Image profile;
    public Text npcName;
    public Text text;

    private string targetDialog;

    private float interval = 1.0f;

    public int dialogSpeed = 20;
    private int curTextIndex = 0;

    void Awake()
    {
        textBar.SetActive(false);
        ButtonOnAndOff(false);
    }

    public void ButtonOnAndOff(bool _onoff)
    {
        if (_onoff)
        {
            pressimage.SetActive(true);
        }
        else
        {
            pressimage.SetActive(false);
        }
    }

    public void SetProfile(Sprite _profile, string _name)
    {
        ButtonOnAndOff(true);

        npcName.text = _name;
        profile.sprite = _profile;
    }

    public void DialogFinish(int _eventnum)
    {
        if(_eventnum == -1)
        {
            GameObject.Find("Player").GetComponent<Player>().isconv = false;

            textBar.SetActive(false);
            ButtonOnAndOff(false);

            ScenesManager.Instance.isimtalking = false;
            ScenesManager.Instance.isOnChoice = false;
        }
        else if(_eventnum > -1)
        {
            if (!ScenesManager.Instance.isOnChoice)
            {
                ScenesManager.Instance.isOnChoice = true;

                List<DialogEventData> eventDataList = ResourceManager.Instance.GetDialogEvent(_eventnum);

                if(eventDataList[0].chice != "")
                {
                    GameObject prefab = Resources.Load("Prefab/UI/OptionButton") as GameObject;

                    foreach (DialogEventData _data in eventDataList)
                    {
                        GameObject _button = Instantiate(prefab, transform.position, Quaternion.identity);
                        _button.transform.SetParent(buttonContent.transform);

                        _button.transform.Find("Text").GetComponent<Text>().text = _data.chice;

                        DialogButton _nextdialog = _button.GetComponent<DialogButton>();
                        _nextdialog.dialogPoint = _data.nextPoint;
                        _nextdialog.dialogIndex = _data.nextDialogIndex;
                        _nextdialog.myDialog = _data.nextDialog;
                        _nextdialog.reward = _data.dialogReward;
                    }
                }
                else if (eventDataList[0].nextDialog[0] != "")
                {
                    ScenesManager.Instance.SetDialogPointInfo(-1, 0);

                    ScenesManager.Instance.curMyDialogArr = eventDataList[0].nextDialog;
                    ScenesManager.Instance.MonologueProcess();
                    ScenesManager.Instance.isimtalking = true;
                }
            }
        }
        else
        {
            ScenesManager.Instance.isimtalking = false;
        }
    }

    public void DialogAnimProcess(string _dialog)
    {
        textBar.SetActive(true);

        if (ScenesManager.Instance.isDialogAnim)
        {
            text.text = _dialog;

            CancelInvoke();

            ScenesManager.Instance.curDialogCount++;
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
            ScenesManager.Instance.curDialogCount++;
            ScenesManager.Instance.isDialogAnim = false;
            return;
        }

        text.text += targetDialog[curTextIndex];
        curTextIndex++;

        Invoke("DialogAimation", interval);
    }
}
