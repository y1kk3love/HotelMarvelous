using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Singleton;

public class ScenesManager : MonoSingleton<ScenesManager>
{
    public GameObject pauseUI;
    public DialogUI dialogUI;
    public OptionInfo optionInfo = new OptionInfo();

    public byte curDialogIndex = 0;

    string[] curDialogarr;

    private bool isDialog = false;
    public bool isDialogAnim = false;
    public bool isOption = false;

    #region [Dialog]

    public void DialogEnter(int _point)
    {
        GameObject _ui = GameObject.Find("DialogUI");

        if (_ui == null)
        {
            GameObject _uiprefab = Resources.Load("Prefab/UI/DialogUI") as GameObject;

            _ui = Instantiate(_uiprefab, transform.position, Quaternion.identity);
            dialogUI = _ui.GetComponent<DialogUI>();
        }

        Debug.Log("Image/DialogProfile/" + _point);

        Sprite _sprite = Resources.Load<Sprite>("Image/DialogProfile/Profile_" + _point);
        dialogUI.SetProfile(_sprite);
    }

    public void DialogProcess(DIALOGZONE _point, int _index)
    {
        if (!isDialog)
        {
            curDialogarr = ResourceManager.instance.GetDialog(_point, _index);

            isDialog = true;
            
            curDialogIndex = 0; 
        }

        if(curDialogIndex + 1 <= curDialogarr.Length)
        {
            dialogUI.DialogAnimProcess(curDialogarr[curDialogIndex]);
        }
        else
        {
            int eventnum = ResourceManager.instance.GetDialogEvent(_point, _index);

            dialogUI.DialogFinish(eventnum);

            isDialog = false;
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
