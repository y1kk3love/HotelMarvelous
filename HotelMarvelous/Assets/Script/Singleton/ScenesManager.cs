using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Singleton;

public class ScenesManager : MonoSingleton<ScenesManager>
{
    public GameObject pauseUI;
    public DialogUI dialogUI;
    public OptionInfo optionInfo = new OptionInfo();

    public byte curDialogCount = 0;

    private int curDialogPoint = -1;
    private int curDialogIndex = -1;

    private string[] curDialogArr;
    public string[] curMyDialogArr;

    private bool isDialog = false;
    public bool isDialogAnim = false;
    public bool isOption = false;
    public bool isOnChoice = false;
    public bool isimtalking = false;

    #region [Dialog]

    public void SetDialogPointInfo(int _point, int _index)
    {
        curDialogPoint = _point;
        curDialogIndex = _index;
    }

    public void DialogEnter(int _point)
    {
        GameObject _ui = GameObject.Find("DialogUI(Clone)");

        if (_ui == null)
        {
            GameObject _uiprefab = Resources.Load("Prefab/UI/DialogUI") as GameObject;

            _ui = Instantiate(_uiprefab, transform.position, Quaternion.identity);
            dialogUI = _ui.GetComponent<DialogUI>();
        }

        DialogProfileChanger(curDialogPoint);
    }

    public void DialogProfileChanger(int _point)
    {
        int npcindex = _point;

        if (isimtalking)
        {
            npcindex = -1;
        }

        Debug.Log("Image/DialogProfile/" + npcindex);

        Sprite _sprite = Resources.Load<Sprite>("Image/DialogProfile/Profile_" + npcindex);
        dialogUI.SetProfile(_sprite);
    }

    public void DialogProcess()
    {
        if (curDialogPoint != -1)
        {
            if (!isDialog)
            {
                curDialogArr = ResourceManager.instance.GetDialog(curDialogPoint, curDialogIndex);

                isDialog = true;

                curDialogCount = 0;
            }

            if (curDialogCount + 1 <= curDialogArr.Length)
            {
                dialogUI.DialogAnimProcess(curDialogArr[curDialogCount]);
            }
            else
            {
                int eventnum = ResourceManager.instance.GetDialogEvent(curDialogPoint, curDialogIndex);

                dialogUI.DialogFinish(eventnum);

                isDialog = false;
            }
        }
    }

    public void MonologueProcess()
    {
        DialogProfileChanger(-1);

        if (!isDialog)
        {
            isDialog = true;

            curDialogCount = 0;
        }

        if (curDialogCount + 1 <= curMyDialogArr.Length)
        {
            dialogUI.DialogAnimProcess(curMyDialogArr[curDialogCount]);
        }
        else
        {
            dialogUI.DialogFinish(-2);

            isDialog = false;

            DialogProfileChanger(curDialogPoint);
            DialogProcess();
        }
    }

    public void EntranceDecision(bool _inout)
    {
        dialogUI.ButtonOnAndOff(_inout);
    }

    #endregion

    #region [SceneMove]

    public void MoveToScene(string _scenename)
    {
        SceneManager.LoadScene(_scenename);
    }

    #endregion

    #region [Option]

    public void ShowPauseButton()
    {
        pauseUI = Resources.Load<GameObject>("Prefab/UI/UI_Pause");

        GameObject option = Instantiate(pauseUI, new Vector3(0, 0, 0), Quaternion.identity);
        option.transform.parent = GameObject.Find("UI").transform;

        Debug.Log("설정버튼 생성완료");

        SetStartScreenOption();
    }

    public void SetStartScreenOption()
    {
        string _resolution = optionInfo.resolution.ToString();
        string _horizontal = _resolution.Substring(1, 4);
        string _vertical = _resolution.Substring(6);

        Debug.Log(_horizontal + " X " + _vertical);

        Screen.SetResolution(int.Parse(_horizontal), int.Parse(_vertical), FullScreen());
    }

    public FullScreenMode FullScreen()
    {
        if (optionInfo.fullscreen == FULLSCREENS.FULLSCREEN)
        {
            return FullScreenMode.FullScreenWindow;
        }
        else
        {
            return FullScreenMode.Windowed;
        }
    }

    #endregion
}

public class OptionInfo
{
    public RESOULUTION resolution = RESOULUTION.R1920X1080;
    public FULLSCREENS fullscreen = FULLSCREENS.FULLSCREEN;

    public KeyCode recharge = KeyCode.Space;
    public KeyCode disposable = KeyCode.LeftControl;
    public KeyCode run = KeyCode.LeftShift;
    public KeyCode minimap = KeyCode.Tab;
    public KeyCode pause = KeyCode.Escape;
    public KeyCode treasure = KeyCode.T;
}
