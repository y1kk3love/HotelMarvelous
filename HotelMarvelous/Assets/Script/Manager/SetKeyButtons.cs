using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetKeyButtons : PauseUI
{
    public KEYSETBUTTON keyset;
    public string stringkey;

    void Start()
    {
        KeySetButtonList.Add(gameObject);
        stringkey = transform.Find("Text").GetComponent<Text>().text;
    }

    public void OnclickSetKeybutton()
    {
        stringkey = transform.Find("Text").GetComponent<Text>().text;
        base.ketsetbutton = keyset;
        base.buttonSetKey = transform.gameObject;
        base.OnclickSetKey();
    }

    public void CancelOption()
    {
        transform.Find("Text").GetComponent<Text>().text = stringkey;
    }
}
