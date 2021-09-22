using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    private GameObject circleFadeout;

    private bool isCircleFade = false;

    void Start()
    {
        circleFadeout = GameObject.Find("CircleOut");
    }

    void FixedUpdate()
    {
        if (isCircleFade)
        {
            CircleOut();
        }
    }

    public void OnClickNewGame()
    {
        isCircleFade = true;
    }

    private void CircleOut()
    {
        if (circleFadeout.transform.localScale.x < 35f)
        {
            circleFadeout.transform.localScale += new Vector3(1, 1, 0);
        }
        else
        {
            ScenesManager.Instance.MoveToScene("Lobby");
        }
    }
}
