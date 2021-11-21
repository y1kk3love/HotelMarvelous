using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum FADE : byte
{
    OUT,
    In
}

enum INTRO : int
{
    PROLOGUETITLE,
    PROLOGUECOMMENT,
    MAINTITLE,
    CHECKIN
}

public class TitleUI : MonoBehaviour
{
     private GameObject startGameui;

    public GameObject[] Intro;
    public GameObject fadeout;

    public Image imTitle;
    public Image imTitleLogo;

    private bool isSkip = false;

    void Start()
    {
        ResourceManager.Instance.LoadResources();        //리소스 매니저 생성

        StartCoroutine(IntroProcess());

        ScenesManager.Instance.ShowPauseButton();
    }

    //인트로 순서 관리
    #region ------------------------------[IntroProcess]------------------------------

    IEnumerator IntroProcess()
    {
        Intro[(int)INTRO.PROLOGUETITLE].SetActive(true);

        yield return new WaitForSeconds(3f);

        Intro[(int)INTRO.PROLOGUETITLE].SetActive(false);
        Intro[(int)INTRO.PROLOGUECOMMENT].SetActive(true);

        yield return new WaitForSeconds(15f);

        Intro[(int)INTRO.PROLOGUECOMMENT].SetActive(false);
        Intro[(int)INTRO.MAINTITLE].SetActive(true);

        StartCoroutine(FadeInOut(imTitle, FADE.In, 4f));
        StartCoroutine(FadeInOut(imTitleLogo, FADE.In, 4f));

        yield return new WaitForSeconds(4f);

        StartCoroutine(FadeInOut(imTitle, FADE.OUT, 4f));
        StartCoroutine(FadeInOut(imTitleLogo, FADE.OUT, 4f));

        yield return new WaitForSeconds(4f);

        Intro[(int)INTRO.MAINTITLE].SetActive(false);
        Intro[(int)INTRO.CHECKIN].SetActive(true);
    }

    #endregion

    //버튼관리
    #region ------------------------------[OnClick]------------------------------

    public void Test_SkipToLobby()
    {
        ScenesManager.Instance.MoveToScene(INTERACTION.LOBBY);
    }

    public void Test_SkipToDungeon()
    {
        ScenesManager.Instance.MoveToScene(INTERACTION.DUNGEON);
    }

    public void CheckIn()
    {
        Instantiate(fadeout);
    }

    #endregion

    //페이드인아웃 관리
    #region ------------------------------[UIEffect]------------------------------

    //들어간 게임오브젝트가 어떤형식의 이미지인지 확인해서 페이드 인 아웃
    IEnumerator FadeInOut(Image _image, FADE _inout, float _time)
    {
        float resettime = Time.time;

        Color color;
        color = _image.color;

        while (color.a < 1 || color.a > 0)
        {   //러프로 부드럽게 페이드 관리
            color.a = Mathf.Lerp(1 - (byte)_inout, (byte)_inout, (Time.time - resettime) / _time);   //이미지의 알파가 1이면 페이드아웃 0이면 페이드인

            _image.color = color;

            if (color.a == (byte)_inout)
            {
                break;
            }

            yield return null;
        }

        if (isSkip)
        {
            imTitle.gameObject.SetActive(false);
        }
    }

    #endregion
}
