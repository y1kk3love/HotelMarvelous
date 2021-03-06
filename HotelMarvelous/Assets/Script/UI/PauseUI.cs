using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    private OptionInfo optionInfo = new OptionInfo();
    protected KEYSETBUTTON ketsetbutton;

    public GameObject panelPause;
    public GameObject panelButtons;
    public GameObject panelOption;
    public GameObject panelDisplay;
    public GameObject scrollviewControl;
    public GameObject panelSound;
    protected GameObject buttonSetKey;
    protected static List<GameObject> KeySetButtonList = new List<GameObject>();

    private static Text textSetKeyInfo;

    public Dropdown dropdownResolution;
    public Dropdown dropdownFullScreen;

    private bool isChangingKey = false;
    private bool isPause = false;

    void Start()
    {
        textSetKeyInfo = GameObject.Find("Text_Info").GetComponent<Text>();

        optionInfo = ScenesManager.Instance.optionInfo;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPause)
            {
                isPause = true;

                OnclickbuttonPause();
            }
            else
            {
                isPause = false;
                OnclickbuttonCountinue();
                OnclickCancel();
            }
        }
    }

    void OnGUI()
    {
        Event KeyEvent = Event.current;

        if (KeyEvent.isKey && isChangingKey && buttonSetKey != null)
        {
            switch (ketsetbutton)
            {
                case KEYSETBUTTON.RECHARGE:
                    ScenesManager.Instance.optionInfo.recharge = KeyEvent.keyCode;
                    break;
                case KEYSETBUTTON.DISPOSABLE:
                    ScenesManager.Instance.optionInfo.disposable = KeyEvent.keyCode;
                    break;
                case KEYSETBUTTON.RUN:
                    ScenesManager.Instance.optionInfo.run = KeyEvent.keyCode;
                    break;
                case KEYSETBUTTON.MINIMAP:
                    ScenesManager.Instance.optionInfo.minimap = KeyEvent.keyCode;
                    break;
                case KEYSETBUTTON.PAUSE:
                    ScenesManager.Instance.optionInfo.pause = KeyEvent.keyCode;
                    break;
                case KEYSETBUTTON.TREASURE:
                    ScenesManager.Instance.optionInfo.treasure = KeyEvent.keyCode;
                    break;
            }

            Debug.Log(KeyEvent.keyCode.ToString());

            buttonSetKey.transform.Find("Text").GetComponent<Text>().text = KeyEvent.keyCode.ToString();
            isChangingKey = false;

            textSetKeyInfo.text = "";
        }
    }

    #region [PauseButtons]

    public void OnclickbuttonPause()
    {
        panelPause.SetActive(true);
        panelButtons.SetActive(true);

        ScenesManager.Instance.isOption = true;
        Time.timeScale = 0;
    }

    public void OnclickbuttonCountinue()
    {
        panelPause.SetActive(false);

        ScenesManager.Instance.isOption = false;
        Time.timeScale = 1;
    }

    public void OnclickbuttonRestart()
    {
        Time.timeScale = 1;

        ScenesManager.Instance.isOption = false;
        ScenesManager.Instance.MoveToScene(INTERACTION.DUNGEON);
    }
    public void OnclickbuttonSaveAndQuit()
    {
        ScenesManager.Instance.MoveToScene(INTERACTION.TITLE);
    }

    public void OnclickbuttonOption()
    {
        panelButtons.SetActive(false);
        panelOption.SetActive(true);
    }
    public void OnclickbuttonMoveToMenu()
    {
        Time.timeScale = 1;

        ScenesManager.Instance.isOption = false;
        ScenesManager.Instance.MoveToScene(INTERACTION.MENU);
    }
    public void OnclickbuttonQuit()
    {
        Application.Quit();
    }

    #endregion

    #region [OptionButtons]

    public void OnclickDisplay()
    {
        panelDisplay.SetActive(true);
        scrollviewControl.SetActive(false);
        panelSound.SetActive(false);
    }

    public void OnclickControl()
    {
        panelDisplay.SetActive(false);
        scrollviewControl.SetActive(true);
        panelSound.SetActive(false);
    }

    public void OnclickSound()
    {
        panelDisplay.SetActive(false);
        scrollviewControl.SetActive(false);
        panelSound.SetActive(true);
    }

    public void OnclickConfirm()
    {
        ScenesManager.Instance.optionInfo.resolution = (RESOULUTION)dropdownResolution.value;
        ScenesManager.Instance.optionInfo.fullscreen = (FULLSCREENS)dropdownFullScreen.value;

        panelPause.SetActive(false);
        panelButtons.SetActive(false);
        panelOption.SetActive(false);
        panelDisplay.SetActive(true);
        scrollviewControl.SetActive(false);
        panelSound.SetActive(false);

        ScenesManager.Instance.isOption = false;
        ScenesManager.Instance.SetStartScreenOption();

        Time.timeScale = 1;
    }

    public void OnclickCancel()
    {
        dropdownResolution.value = (byte)optionInfo.resolution;
        dropdownFullScreen.value = (byte)optionInfo.fullscreen;

        foreach (GameObject button in KeySetButtonList)
        {
            button.GetComponent<SetKeyButtons>().CancelOption();
        }

        panelPause.SetActive(false);
        panelButtons.SetActive(false);
        panelOption.SetActive(false);
        panelDisplay.SetActive(true);
        scrollviewControl.SetActive(false);
        panelSound.SetActive(false);

        ScenesManager.Instance.optionInfo = optionInfo;

        ScenesManager.Instance.isOption = false;

        Time.timeScale = 1;
    }

    #endregion

    #region [ControlButtons]

    public void OnclickSetKey()
    {
        isChangingKey = true;
        textSetKeyInfo.text = "키를 입력해 주세요.";
    }

    #endregion
}
