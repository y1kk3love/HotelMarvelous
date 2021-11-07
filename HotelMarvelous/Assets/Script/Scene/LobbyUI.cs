using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    Player player;

    void Start()
    {
        ScenesManager.Instance.ShowPauseButton();

        ScenesManager.Instance.ShowLocation();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        StartCoroutine(player.MoveInIntro(1.5f, new Vector3(0, 0, 1)));
    }
}
