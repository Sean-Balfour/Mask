using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
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

    public void UpdateLives(int Lives)
    {
        LivesImage.sprite = livesSprites[Lives];
    }
}
