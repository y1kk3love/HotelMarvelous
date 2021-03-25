using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    private VideoPlayer videoplayer;
    private GameObject startgameui;
    private GameObject circleout;
    private Image imtitle;
    private RawImage improlog;

    private bool isvideoplaying = false;
    private bool isskip = false;
    private bool ischeckin = false;

    void Awake()
    {
        ResetVideo();

        ResetGameObject();
        
        StartCoroutine(IntroProcess(imtitle, improlog));
    }
    void Update()
    {
        SkipInput();

        RoundOut();
    }

    #region ------------------------------[OnClick]------------------------------

    public void CheckIn()
    {
        ischeckin = true;
    }

    #endregion

    #region ----------------------------[ResetProcess]----------------------------

    private void ResetGameObject()
    {
        startgameui = GameObject.FindGameObjectWithTag("GameStart");
        imtitle = GameObject.FindGameObjectWithTag("Title").GetComponent<Image>();
        improlog = GameObject.FindGameObjectWithTag("PrologVideo").GetComponent<RawImage>();
        circleout = GameObject.FindGameObjectWithTag("CircleOut");

        startgameui.SetActive(false);
        improlog.enabled = false;
    }

    private void ResetVideo()
    {
        videoplayer = this.gameObject.GetComponent<VideoPlayer>();
        videoplayer.Stop();
    }

    #endregion

    #region ------------------------------[UIEffect]------------------------------

    private void SkipInput()
    {
        if (Input.anyKey && isvideoplaying && !isskip)
        {
            isskip = true;
            SkipVideo();
        }
    }

    private void RoundOut()
    {
        if (ischeckin)
        {
            circleout.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);

            if (circleout.transform.localScale.x > 35f)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

    private void SkipVideo()
    {
        startgameui.SetActive(true);
        isvideoplaying = false;
        videoplayer.Pause();
        StartCoroutine(FadeInOut(null, improlog, 0));
    }

    IEnumerator IntroProcess(Image _image, RawImage _rawimage)
    {
        StartCoroutine(FadeInOut(_image, null, 1));

        yield return new WaitForSeconds(5f);

        videoplayer.Play();

        StartCoroutine(FadeInOut(_image, null, 0));
     
        improlog.enabled = true;

        yield return new WaitForSeconds(3f);

        isvideoplaying = true;

        yield return new WaitForSeconds((float)videoplayer.clip.length);

        if (!isskip)
        {
            SkipVideo();
        }
    }

    //_int --> 1 = FadeIn --> 0 = FadeOut
    IEnumerator FadeInOut(Image _image, RawImage _rawimage, int _int)
    {
        if(_rawimage != null)
        {
            float resettime = Time.time;
            Color color = _rawimage.color;

            while (color.a < 1 || color.a > 0)
            {
                color.a = Mathf.Lerp(1 - _int, _int, (Time.time - resettime) * 0.4f);
                _rawimage.color = color;

                if (color.a == _int)
                {
                    break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            if (isskip)
            {
                GameObject.FindGameObjectWithTag("Title").SetActive(false);
                GameObject.FindGameObjectWithTag("PrologVideo").SetActive(false);
            }
        }
        else if(_image != null)
        {
            float resettime = Time.time;
            Color color = _image.color;

            while (color.a < 1 || color.a > 0)
            {
                color.a = Mathf.Lerp(1 - _int, _int, (Time.time - resettime) * 0.4f);
                _image.color = color;

                if (color.a == _int)
                {
                    break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(4f);
        }
    }

    #endregion
}
