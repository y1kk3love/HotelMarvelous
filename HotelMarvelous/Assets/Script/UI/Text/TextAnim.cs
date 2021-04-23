using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnim : MonoBehaviour
{
    public byte charPerSec = 2;
    private byte index;
    
    private string targetMsg;
    
    private float interval;

    private bool isAnim = false;

    private Text msgText;

    void Awake()
    {
        msgText = GetComponent<Text>();
    }

    public void SetMessage(string _msg)
    {
        if (isAnim)
        {
            msgText.text = targetMsg;

            CancelInvoke();

            TextEffectEnd();
        }
        else
        {
            targetMsg = _msg;

            TextEffectStart();
        }
    }

    private void TextEffectStart()
    {
        msgText.text = "";
        index = 0;

        interval = 1.0f / charPerSec;
        isAnim = true;
       
        Invoke("TextEffecting", interval);
    }

    private void TextEffecting()
    {
        if(msgText.text == targetMsg)
        {
            TextEffectEnd();
            return;
        }

        msgText.text += targetMsg[index];
        index++;

        Invoke("TextEffecting", interval);
    }

    private void TextEffectEnd()
    {
        isAnim = false;
    }

    public bool GetIsAnim()
    {
        return isAnim;
    }
    
}
