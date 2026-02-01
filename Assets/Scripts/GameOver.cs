using UnityEngine;
using UnityEngine.UI;
public class GameOver : MonoBehaviour
{
    [SerializeField]
    private Image Image;

    [SerializeField]
    private Sprite win;

    [SerializeField]
    private Sprite rareWin;

    [SerializeField]
    private int sigmaRarity = 100000;

    private void Start()
    {
        int randomNumber = Random.Range(0, sigmaRarity);
        if (randomNumber == 11)
        {
            Image.sprite = rareWin;
        }
        else
        {
            Image.sprite = win;
        }

        Image.SetNativeSize();
    }
}
