using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AskToMoveUI : MonoBehaviour
{
    public INTERACTION scene;
    public bool isNextStage = false;
    public Text text;
    public Text yestext;
    public Text notext;

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
        else
        {
            if (isNextStage)
            {
                text.text = "지금까지 호텔 마블러스 데모를 플레이 해주셔서 감사합니다.^^";
                yestext.text = "처음으로..";
                notext.text = "남아있기";
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
