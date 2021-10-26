using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject circleFadeout;

    public void OnClickNewGame()
    {
        Instantiate(circleFadeout);
    }
}
