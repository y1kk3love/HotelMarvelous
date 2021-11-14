using UnityEngine;
using Singleton;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesManager : MonoSingleton<ScenesManager>
{
    public GameObject locationInfo;
    public GameObject pauseUI;
    public DialogUI dialogUI;
    public OptionInfo optionInfo = new OptionInfo();

    public byte curDialogCount = 0;

    private int curDialogPoint = -1;
    private int curDialogIndex = -1;
    private int curNpcprofile = -1;

    private string[] curDialogArr;
    public string[] curMyDialogArr;

    public bool isDialog = false;
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
        if(curNpcprofile != _point)
        {
            curNpcprofile = _point;

            if (isimtalking)
            {
                curNpcprofile = -1;
            }

            Sprite _sprite = Resources.Load<Sprite>("Image/DialogProfile/Profile_" + curNpcprofile);

            string _name = "";

            switch (_point)
            {
                case -1:
                    _name = "클라우스 로빈슨";
                    break;
                case 0:
                    _name = "케니스";
                    break;
                case 1:
                    _name = "엘리자베스 테일러";
                    break;
                case 2:
                    _name = "파피";
                    break;
            }

            dialogUI.SetProfile(_sprite, _name);
        }
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

                isDialog = false;

                dialogUI.DialogFinish(eventnum);
            }
        }
        else
        {
            dialogUI.DialogFinish(-1);
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
            isDialog = false;

            dialogUI.DialogFinish(-2);

            DialogProfileChanger(curDialogPoint);

            if(curDialogPoint != -1)
            {
                DialogProcess();
            }
            else
            {
                dialogUI.DialogFinish(-1);

                isOnChoice = false;
                isDialog = false;
            }
        }
    }

    public void EntranceDecision(bool _inout)
    {
        dialogUI.ButtonOnAndOff(_inout);
    }

    #endregion

    #region [SceneMove]

    public string CheckScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void MoveToScene(INTERACTION _scenename)
    {
        string scene = _scenename.ToString();

        if (_scenename == INTERACTION.DUNGEON || _scenename == INTERACTION.LOBBY)
        {
            LoadingSceneController.LoadScene(scene);
        }
        else
        {
            SceneManager.LoadScene(scene);
        }
        
    }

    public void ShowLocation()
    {
        locationInfo = Resources.Load<GameObject>("Prefab/UI/LocationInfo");

        GameObject info = Instantiate(locationInfo, new Vector3(0,0,0), Quaternion.identity);

        info.transform.GetChild(1).GetComponent<Text>().text = SceneManager.GetActiveScene().name;

        Destroy(info, 1.3f);
    }

    #endregion

    #region [Option]

    public void ShowPauseButton()
    {
        pauseUI = Resources.Load<GameObject>("Prefab/UI/UI_Pause");

        GameObject option = Instantiate(pauseUI, new Vector3(0, 0, 0), Quaternion.identity);
        GameObject ui = GameObject.Find("UI");

        if(ui != null)
        {
            option.transform.parent = ui.transform;
        }

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
