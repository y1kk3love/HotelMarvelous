using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    private GameObject buttonimage;
    private GameObject textbar;

    private Player player;
    private TextManager textmanager;

    private bool istalking;

    private int talkIndex;

    private Text talkText;

    void Start()
    {
        ResetUI();
    }

    void Update()
    {
        TextZoneCheck();
    }

    private void TextZoneCheck()
    {
        buttonimage.SetActive(player.GetOnTextZone());

        if (Input.GetKeyDown(KeyCode.E))
        {
            textbar.SetActive(true);
            Debug.Log("작동");

            TalkZoneReset();
        }
    }   

    private void TalkZoneReset()
    {
        Dialoguezone dialoguezone = player.GetObZone().GetComponent<Dialoguezone>();
        TalkProcess(dialoguezone.id);

        textbar.SetActive(istalking);
        buttonimage.SetActive(istalking);
    }

    private void TalkProcess(int id)
    {
        string talkData = textmanager.GetTalk(id, talkIndex);

        if(talkData == null)
        {
            istalking = false;
            player.ExitTextZone();
            talkIndex = 0;
            buttonimage.SetActive(false);
            player.SetIsConversation(false);
            return;
        }

        talkText.text = talkData;

        player.SetIsConversation(true);
        istalking = true;
        talkIndex++;
    }

    private void ResetUI()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        talkText = GameObject.Find("TalkText").GetComponent<Text>();
        textmanager = gameObject.GetComponent<TextManager>();
        buttonimage = GameObject.Find("PressE");
        textbar = GameObject.Find("TextBar");

        buttonimage.SetActive(false);
        textbar.SetActive(false);
    }
}

