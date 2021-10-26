using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AskToMoveUI : MonoBehaviour
{
    public INTERACTION scene;
    public bool isNextStage = false;
    public Text text;

    void Start()
    {
        if(scene == INTERACTION.DUNGEON)
        {
            if (isNextStage)
            {
                text.text = "엘리베이터를 사용하여 다음층으로 이동할까요?";
            }
            else
            {
                text.text = "호텔로 들어가시겠습니까?";
            }
        }
    }

    public void MoveToScene()
    {
        if(scene != INTERACTION.NONE)
        {
            ScenesManager.Instance.MoveToScene(scene);
        }
    }

    public void StayThisScene()
    {
        GameObject.Find("Player").GetComponent<Player>().isconv = false;

        Destroy(transform.gameObject);
    }
}
