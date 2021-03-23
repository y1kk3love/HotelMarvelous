using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private GameObject circleout;

    private bool isnewgameon = false;

    void Start()
    {
        circleout = GameObject.FindGameObjectWithTag("CircleOut");
    }

    public void OnClickNewGame()
    {
        isnewgameon = true;
    }

    void Update()
    {
        if (isnewgameon)
        {
            circleout.transform.localScale += new Vector3(50f, 50f, 50f) * Time.deltaTime;

            if (circleout.transform.localScale.x > 35)
            {
                isnewgameon = false;

                SceneManager.LoadScene("Lobby");
            }
        }
        
    }
}
