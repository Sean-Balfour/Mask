using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
    }

    [SerializeField]
    private List<Sprite> livesSprites = new List<Sprite>();

    [SerializeField]
    private Image LivesImage;

    [SerializeField]
    private RectTransform meterBar;
    [SerializeField]
    private float meterBarMaxHeight = 887;

    [SerializeField]
    private Player player;

    [SerializeField]
    private Transform start;

    [SerializeField]
    private Goal goal;

    [SerializeField]
    private float shrimpLeft = 140.0f;

    [SerializeField]
    private float shrimpRight = 1800.0f;

    [SerializeField]
    private Image shrimpProgress;

    private void Update()
    {
        if (start == null)
        {
            return;
        }

        float currentDistance = Vector3.Distance(start.transform.position, player.transform.position);
        float totalDistance = Vector3.Distance(start.transform.position, goal.transform.position);

        float progress = Mathf.Clamp01(currentDistance / totalDistance);

        Debug.Log(progress);

        float currentPosition = Mathf.Lerp(shrimpLeft, shrimpRight, progress);
        Debug.Log(currentPosition);

        shrimpProgress.rectTransform.anchoredPosition = new Vector3(currentPosition, 380.0f, 0.0f);
    }

    public void UpdateLives(int Lives)
    {
        LivesImage.sprite = livesSprites[Lives];
    }

    public void UpdateMeterBar(float meterNormalized)
    {
        meterBar.sizeDelta = new Vector2(249, (meterBarMaxHeight * meterNormalized));
    }

    public void PlayGame()
    {
        StartCoroutine(PlayGameFade());
    }

    private IEnumerator PlayGameFade()
    {
        Fade.Instance.FadeOut();

        float tick = 0;
        for (int i = 0; i < 100; i++)
        {
            tick++;
            yield return new WaitForSeconds(1.0f / 100.0f);
        }

        SceneManager.LoadScene("Level_1");
    }
    public void RestartGame()
    {
        StartCoroutine(RestartGameFade());
    }

    private IEnumerator RestartGameFade()
    {
        Fade.Instance.FadeOut();

        float tick = 0;
        for (int i = 0; i < 100; i++)
        {
            tick++;
            yield return new WaitForSeconds(1.0f / 100.0f);
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        StartCoroutine(QuitGameFade());
    }

    private IEnumerator QuitGameFade()
    {
        Fade.Instance.FadeOut();

        float tick = 0;
        for (int i = 0; i < 100; i++)
        {
            tick++;
            yield return new WaitForSeconds(1.0f / 100.0f);
        }

        Application.Quit();
    }
}
