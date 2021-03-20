using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    private GameObject obtitle;
    private GameObject obprolog;

    private Image imtitle;

    void Awake()
    {
        obtitle = GameObject.FindGameObjectWithTag("Title");
        imtitle = obtitle.GetComponent<Image>();
        obprolog = GameObject.FindGameObjectWithTag("PrologVideo");
        obprolog.SetActive(false);
    }

    void Start()
    {
        Color color = imtitle.color;
        color.a = 0;
    }

    void Update()
    {
        
    }
}
