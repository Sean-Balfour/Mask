using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField]
    private Image FadeBG;

    private void Awake()
    {
        FadeIn();
    }

    public void FadeIn()
    {
        StartCoroutine(DoFadeIn());
    }

    public void FadeOut()
    {
        StartCoroutine(DoFadeOut());
    }

    public void FadeOutEnd()
    {
        StartCoroutine(DoFadeOutEnd());
    }

    private IEnumerator DoFadeOut()
    {
        float tick = 0;
        for (int i = 0; i < 100; i++)
        {
            tick++;
            yield return new WaitForSeconds(1.0f / 100.0f);
            FadeBG.color = new Color(0.0f, 0.0f, 0.0f, tick / 100.0f);
        }
    }

    private IEnumerator DoFadeOutEnd()
    {
        float tick = 0;
        for (int i = 0; i < 100; i++)
        {
            tick++;
            yield return new WaitForSeconds(1.0f / 100.0f);
            FadeBG.color = new Color(0.0f, 0.0f, 0.0f, tick / 100.0f);
        }

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator DoFadeIn()
    {
        FadeBG.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        yield return new WaitForSeconds(0.5f);
        float tick = 0;
        for (int i = 0; i < 100; i++)
        {
            tick++;
            yield return new WaitForSeconds(1.0f / 100.0f);
            FadeBG.color = new Color(0.0f, 0.0f, 0.0f, 1.0f - (tick / 100.0f));
        }
    }
}
