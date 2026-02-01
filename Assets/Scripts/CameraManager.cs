using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    

    [SerializeField]
    private Vector3 startPosition;

    [SerializeField]
    private Vector3 endPosition;

    [SerializeField]
    private float timeToMove = 60.0f;

    private float timeTaken;

    private float timeToStart = 5.0f;

    private bool moving = false;

    private void Awake()
    {
        transform.position = startPosition;

        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartMoving());
    }

    private IEnumerator StartMoving()
    {
        yield return new WaitForSeconds(timeToStart);

        moving = true;
    }

    public void ResetCamera()
    {
        timeTaken = 0.0f;
        transform.position = startPosition;
    }

    private void FixedUpdate()
    {
        if (timeTaken >= timeToMove || !moving)
        {
            return;
        }

        timeTaken += Time.fixedDeltaTime;

        transform.position = Vector3.Lerp(startPosition, endPosition, (timeTaken / timeToMove));
    }
}
