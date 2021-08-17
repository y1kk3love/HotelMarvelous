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

    private byte talkIndex;

    private TextAnim textanim;

    void Start()
    {
        //ResetUI();
    }

    void Update()
    {
        //TextZoneCheck();
    }

    /*
    private void TextZoneCheck()
    {
        GameObject obtextzone = player.GetObZone();

        if (obtextzone != null && obtextzone.GetComponent<Dialoguezone>().textzone != TEXTZONE.TODUNGEON)
        {
            buttonimage.SetActive(player.GetOnTextZone());
        }       

        if (Input.GetKeyDown(KeyCode.E) && player.GetOnTextZone())
        {
            textbar.SetActive(true);
            Debug.Log("작동");

            TalkZoneReset();
        }
    }   

    private void TalkZoneReset()
    {
        Dialoguezone dialoguezone = player.GetObZone().GetComponent<Dialoguezone>();
        TalkProcess((byte)dialoguezone.textzone , dialoguezone.Dialogueid);

        textbar.SetActive(istalking);
        buttonimage.SetActive(istalking);
    }

    private void TalkProcess(byte _zonename, byte _id)
    {
        string talkData = "";

        if (textanim.GetIsAnim())
        {
            textanim.SetMessage("");
            return;
        }
        else
        {
            talkData = textmanager.GetTalk(_zonename, _id, talkIndex);
        }

        if(talkData == null)
        {
            istalking = false;
            player.ExitTextZone();
            talkIndex = 0;
            buttonimage.SetActive(false);
            player.SetIsConversation(false);
            return;
        }

        textanim.SetMessage(talkData);

        player.SetIsConversation(true);
        istalking = true;
        talkIndex++;
    }

    private void ResetUI()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        textanim = GameObject.Find("TalkText").GetComponent<TextAnim>();
        textmanager = gameObject.GetComponent<TextManager>();
        buttonimage = GameObject.Find("PressE");
        textbar = GameObject.Find("TextBar");

        buttonimage.SetActive(false);
        textbar.SetActive(false);

        ScenesManager.Instance.ShowPauseButton();
    }
    */
}

