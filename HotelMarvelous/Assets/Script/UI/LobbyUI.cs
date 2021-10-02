using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    void Start()
    {
        ScenesManager.Instance.ShowPauseButton();

        ScenesManager.Instance.ShowLocation();
    }
}
