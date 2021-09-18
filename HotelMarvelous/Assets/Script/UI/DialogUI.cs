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

    public void SetProfile(Sprite _profile)
    {
        ButtonOnAndOff(true);

        profile.sprite = _profile;
    }

    public void DialogFinish(int _eventnum)
    {
        if(_eventnum == -1)
        {
            GameObject.Find("Player").GetComponent<Player>().isconv = false;

            textBar.SetActive(false);
            ButtonOnAndOff(false);
        }
        else
        {
            if (!ScenesManager.Instance.isOnChoice)
            {
                ScenesManager.Instance.isOnChoice = true;

                List<DialogEventData> eventDataList = ResourceManager.Instance.GetDialogEvent(_eventnum);

                GameObject prefab = Resources.Load("Prefab/UI/OptionButton") as GameObject;

                foreach (DialogEventData data in eventDataList)
                {
                    GameObject button = Instantiate(prefab, transform.position, Quaternion.identity);
                    button.transform.parent = buttonContent.transform;

                    button.transform.Find("Text").GetComponent<Text>().text = data.chice;

                    DialogButton nextdialog = button.GetComponent<DialogButton>();
                    nextdialog.dialogPoint = data.nextPoint;
                    nextdialog.dialogIndex = data.nextDialogIndex;
                }

                Debug.Log("이벤트 가능");
            }
        }
    }

    public void DialogAnimProcess(string _dialog)
    {
        textBar.SetActive(true);

        if (ScenesManager.Instance.isDialogAnim)
        {
            text.text = _dialog;

            CancelInvoke();

            ScenesManager.Instance.curDialogIndex++;
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
            ScenesManager.Instance.curDialogIndex++;
            ScenesManager.Instance.isDialogAnim = false;
            return;
        }

        text.text += targetDialog[curTextIndex];
        curTextIndex++;

        Invoke("DialogAimation", interval);
    }
}
