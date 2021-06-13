using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    private OptionInfo optionInfo = new OptionInfo();
    protected KEYSETBUTTON ketsetbutton;

    private  GameObject panelPause;
    private GameObject panelButtons;
    private GameObject panelOption;
    private GameObject panelDisplay;
    private GameObject scrollviewControl;
    private GameObject panelSound;
    protected GameObject buttonSetKey;
    protected static List<GameObject> KeySetButtonList = new List<GameObject>();

    private Text textRecharge;
    private Text textDisposable;
    private Text textRun;
    private Text textMiniMap;
    private Text textPause;
    private Text textTreasure;

    private static Text textSetKeyInfo;

    private Dropdown dropdownResolution;
    private Dropdown dropdownFullScreen;

    private KeyCode recharge;
    private KeyCode disposable;
    private KeyCode run;
    private KeyCode minimap;
    private KeyCode pause;
    private KeyCode treasure;

    private bool isChangingKey = false;

    void Start()
    {
        panelPause = GameObject.Find("Panel_Pause").gameObject;
        panelButtons = GameObject.Find("Panel_Buttons").gameObject;
        panelOption = GameObject.Find("Panel_Option").gameObject;
        panelDisplay = GameObject.Find("Panel_Display").gameObject;
        scrollviewControl = GameObject.Find("ScrollView_Control").gameObject;
        panelSound = GameObject.Find("Panel_Sound").gameObject;
        dropdownResolution = GameObject.Find("Dropdown_Resolution").GetComponent<Dropdown>();
        dropdownFullScreen = GameObject.Find("Dropdown_FullScreen").GetComponent<Dropdown>();

        textSetKeyInfo = GameObject.Find("Text_SetKey").GetComponent<Text>();

        panelButtons.SetActive(false);
        textSetKeyInfo.text = "";
        scrollviewControl.SetActive(false);
        panelSound.SetActive(false);
        panelPause.SetActive(false);
        panelOption.SetActive(false);
    }

    void OnGUI()
    {
        Event KeyEvent = Event.current;

        if (KeyEvent.isKey && isChangingKey && buttonSetKey != null)
        {
            switch (ketsetbutton)
            {
                case KEYSETBUTTON.RECHARGE:
                    recharge = KeyEvent.keyCode;
                    break;
                case KEYSETBUTTON.DISPOSABLE:
                    disposable = KeyEvent.keyCode;
                    break;
                case KEYSETBUTTON.RUN:
                    run = KeyEvent.keyCode;
                    break;
                case KEYSETBUTTON.MINIMAP:
                    minimap = KeyEvent.keyCode;
                    break;
                case KEYSETBUTTON.PAUSE:
                    pause = KeyEvent.keyCode;
                    break;
                case KEYSETBUTTON.TREASURE:
                    treasure = KeyEvent.keyCode;
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
        Time.timeScale = 0;
    }

    public void OnclickbuttonCountinue()
    {
        panelPause.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnclickbuttonRestart()
    {
        Time.timeScale = 1;
        ScenesManager.Instance.MoveToScene("Dungeon");
    }
    public void OnclickbuttonSaveAndQuit()
    {
        //저장 기능 추가
        Application.Quit();
    }

    public void OnclickbuttonOption()
    {
        panelButtons.SetActive(false);
        panelOption.SetActive(true);
    }
    public void OnclickbuttonMoveToMenu()
    {
        Time.timeScale = 1;
        ScenesManager.Instance.MoveToScene("Menu");
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
        optionInfo.resolution = (byte)dropdownResolution.value;
        optionInfo.fullscreen = (byte)dropdownFullScreen.value;

        optionInfo.recharge = recharge;
        optionInfo.disposable = disposable;
        optionInfo.run = run;
        optionInfo.minimap = minimap;
        optionInfo.pause = pause;
        optionInfo.treasure = treasure;

        panelPause.SetActive(false);
        panelButtons.SetActive(false);
        panelOption.SetActive(false);
        panelDisplay.SetActive(true);
        scrollviewControl.SetActive(false);
        panelSound.SetActive(false);

        ScenesManager.Instance.optionInfo = optionInfo;
        Time.timeScale = 1;
    }

    public void OnclickCancel()
    {
        dropdownResolution.value = optionInfo.resolution;
        dropdownFullScreen.value = optionInfo.fullscreen;

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

public class OptionInfo
{
    public byte resolution = (byte)RESOULUTION.R1920X1080;
    public byte fullscreen = (byte)FULLSCREENS.FULLSCREEN;

    public KeyCode recharge = KeyCode.Space;
    public KeyCode disposable = KeyCode.LeftControl;
    public KeyCode run = KeyCode.LeftShift;
    public KeyCode minimap = KeyCode.Tab;
    public KeyCode pause = KeyCode.Escape;
    public KeyCode treasure = KeyCode.T;
}
