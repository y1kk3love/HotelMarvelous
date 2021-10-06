using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleUI : MonoBehaviour
{
    private enum FADE : byte
    {
        In,
        OUT
    }

    private VideoPlayer videoPlayer;

    private GameObject startGameui;

    private GameObject circleFadeout;
    private Image imTitle;
    private RawImage imProlog;

    private bool isVideoPlaying = false;
    private bool isSkip = false;
    private bool isCircleFade = false;

    void Start()
    {
        ResourceManager.Instance.LoadResources();        //리소스 매니저 생성

        ResetProcess();
        
        StartCoroutine(IntroProcess(imTitle));

        ScenesManager.Instance.ShowPauseButton();
    }

    void FixedUpdate()
    {
        if (isCircleFade)
        {
            CircleOut();
        }
    }

    //인트로 순서 관리
    #region ------------------------------[IntroProcess]------------------------------

    IEnumerator IntroProcess(Image _image)
    {
        StartCoroutine(FadeInOut(_image.gameObject, FADE.OUT));

        yield return new WaitForSeconds(5f);

        videoPlayer.Play();

        StartCoroutine(FadeInOut(_image.gameObject, FADE.In));

        imProlog.enabled = true;

        yield return new WaitForSeconds(3f);

        isVideoPlaying = true;

        while (true)
        {
            if (Input.anyKey && isVideoPlaying && !isSkip)
            {
                isSkip = true;
                SkipVideo();
                break;
            }

            //코루틴을 업데이트 처럼 사용
            yield return null;
        }

        yield return new WaitForSeconds((float)videoPlayer.clip.length);

        if (!isSkip)
        {
            SkipVideo();
        }
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
        isCircleFade = true;
    }

    #endregion

    //값 초기화
    #region ----------------------------[ResetProcess]----------------------------

    private void ResetProcess()
    {   //오브젝트들 초기화
        startGameui = GameObject.Find("StartGame");
        imTitle = GameObject.Find("Title").GetComponent<Image>();
        imProlog = GameObject.Find("PrologVideo").GetComponent<RawImage>();
        circleFadeout = GameObject.Find("CircleOut");

        startGameui.SetActive(false);
        imProlog.enabled = false;

        //비디오 초기화
        videoPlayer = gameObject.GetComponent<VideoPlayer>();
        videoPlayer.clip = Resources.Load("Video/PrologVideo") as VideoClip;
        videoPlayer.Stop();
    }

    #endregion

    //페이드인아웃 관리
    #region ------------------------------[UIEffect]------------------------------

    //원형 페이드 아웃
    private void CircleOut()
    {
        if (circleFadeout.transform.localScale.x < 35f)
        {
            circleFadeout.transform.localScale += new Vector3(1, 1, 0);
        }
        else
        {
            ScenesManager.Instance.MoveToScene(INTERACTION.MENU);
        }
    }

    //비디오 스킵
    private void SkipVideo()
    {
        startGameui.SetActive(true);
        isVideoPlaying = false;
        videoPlayer.Pause();
        StartCoroutine(FadeInOut(imProlog.gameObject, FADE.In));
    }

    //들어간 게임오브젝트가 어떤형식의 이미지인지 확인해서 페이드 인 아웃
    IEnumerator FadeInOut(GameObject _imageobj, FADE _inout)
    {
        float resettime = Time.time;
        Color color;
        Image _image;
        RawImage _rawimage;


        if(_imageobj.GetComponent<Image>() != null)
        {
            _image = _imageobj.GetComponent<Image>();
            color = _image.color;
                
            while (color.a < 1 || color.a > 0)
            {   //러프로 부드럽게 페이드 관리
                color.a = Mathf.Lerp(1 - (byte)_inout, (byte)_inout, (Time.time - resettime) * 0.4f);   //이미지의 알파가 1이면 페이드아웃 0이면 페이드인

                _image.color = color;

                if (color.a == (byte)_inout)
                {
                    break;
                }

                yield return null;
            }
        }
        else if(_imageobj.GetComponent<RawImage>() != null)
        {
            _rawimage = _imageobj.GetComponent<RawImage>();
            color = _rawimage.color;

            while (color.a < 1 || color.a > 0)
            {
                color.a = Mathf.Lerp(1 - (byte)_inout, (byte)_inout, (Time.time - resettime) * 0.4f);

                _rawimage.color = color;

                if (color.a == (byte)_inout)
                {
                    break;
                }

                yield return null;
            }
        }
        else
        {
            Debug.Log("페이드인아웃을 위한 Image 혹은 RawImag 형태의 게임오브젝트를 넣어주세요.");

            yield break;
        }

        if (isSkip)
        {
            imTitle.gameObject.SetActive(false);
            imProlog.gameObject.SetActive(false);
        }
    }

    #endregion
}
