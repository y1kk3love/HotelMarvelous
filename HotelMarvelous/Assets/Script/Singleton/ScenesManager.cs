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
        Instantiate(pauseUI, new Vector3(0, 0, 0), Quaternion.identity);
        Debug.Log("설정버튼 생성완료");
    }
}
