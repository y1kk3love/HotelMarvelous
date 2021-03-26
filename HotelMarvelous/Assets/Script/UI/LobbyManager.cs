using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    private Player player;
    private TextManager textmanager;

    private bool istalking;

    private int talkIndex;

    private Text talkText;
    private List<GameObject> uilist = new List<GameObject>();
    private Dictionary<int, string[]> talkData;

    void Start()
    {
        ResetUI();
    }

    void Update()
    {
        UIProcess();
    }

    private void UIProcess()
    {
        if (player.OnTextZone())
        {
            uilist[0].SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {                
                uilist[1].SetActive(true);
                Debug.Log("작동");

                OntalkZoneEnter();
            }
        }
    }   

    private void OntalkZoneEnter()
    {
        Dialoguezone dialoguezone = player.ObZone().GetComponent<Dialoguezone>();
        Talk(dialoguezone.id);

        uilist[1].SetActive(istalking);
    }

    void Talk(int id)
    {
        string talkData = textmanager.GetTalk(id, talkIndex);

        if(talkData == null)
        {
            istalking = false;
            player.ExitTextZone();
            talkIndex = 0;
            return;
        }

        talkText.text = talkData;

        istalking = true;
        talkIndex++;
    }

    private void ResetUI()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        talkText = GameObject.FindGameObjectWithTag("TalkText").GetComponent<Text>();
        textmanager = gameObject.GetComponent<TextManager>();

        uilist.Add(transform.FindChild("PressE").gameObject);
        uilist.Add(transform.FindChild("TextBar").gameObject);

        for (int i = 0; i < uilist.Count; i++)
        {
            uilist[i].SetActive(false);
            Debug.Log(i);
        }
    }
}

