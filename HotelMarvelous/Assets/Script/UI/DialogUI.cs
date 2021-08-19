using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    public GameObject textBar;
    public GameObject pressimage;
    public Image profile;
    public Text text;

    void Start()
    {
        textBar = GameObject.Find("TextBar");
        pressimage = GameObject.Find("PressE");
        profile = GameObject.Find("DialogProfile").GetComponent<Image>();
        text = GameObject.Find("DialogText").GetComponent<Text>();

        textBar.SetActive(false);
        pressimage.SetActive(false);
    }

    public void SetProfile(Sprite _profile)
    {
        pressimage.SetActive(true);

        profile.sprite = _profile;
    }

    public void SetDialog(string _text)
    {
        text.text = _text;
        textBar.SetActive(true);
    }

    public void DialogFinish()
    {
        GameObject.Find("Player").GetComponent<Player>().isconv = false;

        textBar.SetActive(false);
        pressimage.SetActive(false);
    }
}
