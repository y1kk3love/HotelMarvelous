using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    private GameObject circleFadeout;

    void Start()
    {
        circleFadeout = GameObject.Find("CircleOut");
    }

    public void OnClickNewGame()
    {
        StartCoroutine(CircleOut(0.2f));
    }

    IEnumerator CircleOut(float _speed)
    {
        while (circleFadeout.transform.localScale.x < 35f)
        {
            circleFadeout.transform.localScale += new Vector3(_speed, _speed, 0);

            yield return null;
        }

        SceneManager.LoadScene("Lobby");
    }
}
