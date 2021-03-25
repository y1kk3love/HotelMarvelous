using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    private bool iscounterenter;

    private List<GameObject> uilist = new List<GameObject>();
    private Dictionary<int, string[]> talkData;

    void Start()
    {
        ResetUI();

        ResetText();
    }

    void Update()
    {
        UIProcess();
    }

    public void CounterEnter(bool _bool)
    {
        iscounterenter = _bool;
    }

    private void UIProcess()
    {
        if (iscounterenter)
        {
            uilist[0].SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("작동");
                iscounterenter = false;
                uilist[0].SetActive(false);
            }
        }
    }

    private void ResetUI()
    {
        uilist.Add(transform.FindChild("PressE").gameObject);
        uilist[0].SetActive(false);
    }

    private void ResetText()
    {
        talkData = new Dictionary<int, string[]>();
    }

    private void GenerateData()
    {
        talkData.Add(1000, new string[] { "화나게 하지마 이희원" });
    }
}

