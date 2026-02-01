using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(Fade.Instance.DoFadeOut());
            StartCoroutine(EndLevel());
        }
    }

    private IEnumerator EndLevel()
    {
        float tick = 0;
        for (int i = 0; i < 100; i++)
        {
            tick++;
            yield return new WaitForSeconds(1.0f / 100.0f);
        }

        if (SceneManager.GetActiveScene().buildIndex != 3)
        {
            SceneManager.LoadScene("Level_" + (SceneManager.GetActiveScene().buildIndex + 1).ToString());
        }
        else
        {
            SceneManager.LoadScene("WinScreen");
        }
    }
}
