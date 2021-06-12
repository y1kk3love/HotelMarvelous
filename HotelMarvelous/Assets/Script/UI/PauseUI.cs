using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    private GameObject panelPause;
    private GameObject panelOption;

    void Start()
    {
        panelPause = transform.Find("Panel_Pause").gameObject;
        panelPause.SetActive(false);
    }

    public void OnclickbuttonPause()
    {
        panelPause.SetActive(true);
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
        //옵션추가
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
}
