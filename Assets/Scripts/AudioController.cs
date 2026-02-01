using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    private AudioSource ambience;
    [SerializeField]
    private AudioSource bite;
    [SerializeField]
    private AudioSource buttonPress;

    [SerializeField]
    private AudioClip biteClip;

    [SerializeField]
    private AudioClip buttonPressClip;

    public void Bite()
    {
        bite.PlayOneShot(biteClip);
    }

    public void ButtonPress()
    {
        buttonPress.PlayOneShot(buttonPressClip);
    }
}
