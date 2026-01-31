using UnityEditor.SceneManagement;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Vector3 startPosition;

    [SerializeField]
    private Vector3 endPosition;

    [SerializeField]
    private float timeToMove = 60.0f;

    private float timeTaken;

    private void Awake()
    {
        transform.position = startPosition;
    }

    private void FixedUpdate()
    {
        if (timeTaken >= timeToMove)
        {
            return;
        }

        timeTaken += Time.fixedDeltaTime;

        transform.position = Vector3.Lerp(startPosition, endPosition, (timeTaken / timeToMove));
    }
}
