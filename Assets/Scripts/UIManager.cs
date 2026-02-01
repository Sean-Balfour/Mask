using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [SerializeField]
    private GameObject PauseMenu;

    [SerializeField]
    private GameObject GameMenu;

    [SerializeField]
    private GameObject controlsMenu;

    [SerializeField]
    private GameObject mainMenu;

    [SerializeField] private InputActionReference pauseAction;

    private bool Paused = false;

    private void OnEnable()
    {
        if (pauseAction)
            pauseAction.action.performed += TogglePause;
    }

    private void OnDisable()
    {
        if (pauseAction)
            pauseAction.action.performed -= TogglePause;
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        TogglePauseNoContext();
    }

    public void TogglePauseNoContext()
    {
        if (!PauseMenu)
            return;

        AudioController.Instance.ButtonPress();

        if (Paused)
        {
            Time.timeScale = 1.0f;
            PauseMenu.SetActive(false);
            GameMenu.SetActive(true);
            Paused = false;
        }
        else
        {
            Time.timeScale = 0.0f;
            PauseMenu.SetActive(true);
            GameMenu.SetActive(false);
            Paused = true;
        }
    }

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

    public void ControlsMenu()
    {
        AudioController.Instance.ButtonPress();

        controlsMenu.SetActive(true);
        if (PauseMenu)
            PauseMenu.SetActive(false);
        if (mainMenu)
            mainMenu.SetActive(false);
    }

    public void Back()
    {
        AudioController.Instance.ButtonPress();

        controlsMenu.SetActive(false);
        if (PauseMenu)
            PauseMenu.SetActive(true);
        if (mainMenu)
            mainMenu.SetActive(true);
    }

    public void UpdateLives(int Lives)
    {
        LivesImage.sprite = livesSprites[Lives];
    }

    public void UpdateMeterBar(float meterNormalized)
    {
        meterBar.sizeDelta = new Vector2(249, meterBarMaxHeight * meterNormalized);
    }

    public void PlayGame()
    {
        AudioController.Instance.ButtonPress();
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
        AudioController.Instance.ButtonPress();
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

    public void QuitLevel()
    {
        AudioController.Instance.ButtonPress();
        StartCoroutine(QuitLevelFade());
    }

    private IEnumerator QuitLevelFade()
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
        AudioController.Instance.ButtonPress();
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
