using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using game;

public class ScenesManager : MonoSingleton<ScenesManager>
{
    public GameObject pauseUI;
    public OptionInfo optionInfo = new OptionInfo();

    public bool onOption = false;

    void Start()
    {
        pauseUI = Resources.Load<GameObject>("Prefab/UI/UI_Pause");
    }

    public void MoveToScene(string _scenename)
    {
        SceneManager.LoadScene(_scenename);
    }

    public void ShowPauseButton()
    {
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
}

public class OptionInfo
{
    public RESOULUTION resolution = RESOULUTION.R1920X1080;
    public FULLSCREENS fullscreen = FULLSCREENS.FULLSCREEN;

    public KeyCode recharge = KeyCode.Space;
    public KeyCode disposable = KeyCode.LeftControl;
    public KeyCode run = KeyCode.LeftControl;
    public KeyCode minimap = KeyCode.Tab;
    public KeyCode pause = KeyCode.Escape;
    public KeyCode treasure = KeyCode.T;
}
