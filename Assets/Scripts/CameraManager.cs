using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float smoothTime = 0.18f;

    [Header("Look Ahead")]
    [SerializeField] private bool useLookAhead = true;
    [SerializeField] private float lookAheadDistance = 1.5f;
    [SerializeField] private float lookAheadSmoothTime = 0.12f;
    [SerializeField] private float lookAheadDeadZone = 0.15f;

    private Vector3 _velocity;
    private Vector3 _lookAheadVelocity;
    private Vector3 _currentLookAhead;

    private Player _player;

    private void Awake()
    {
        RefreshPlayerRef();
    }

    private void OnEnable()
    {
        RefreshPlayerRef();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desired = target.position;

        if (useLookAhead && _player != null)
        {
            Vector2 dir = _player.LastMoveDirection;

            Vector3 lookTarget = (dir.sqrMagnitude > lookAheadDeadZone * lookAheadDeadZone)
                ? new Vector3(dir.x, dir.y, 0f) * lookAheadDistance
                : Vector3.zero;

            _currentLookAhead = Vector3.SmoothDamp(
                _currentLookAhead,
                lookTarget,
                ref _lookAheadVelocity,
                lookAheadSmoothTime
            );

            desired += _currentLookAhead;
        }

        desired += offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref _velocity,
            smoothTime
        );
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        RefreshPlayerRef();
        _velocity = Vector3.zero;
        _lookAheadVelocity = Vector3.zero;
        _currentLookAhead = Vector3.zero;
    }

    private void RefreshPlayerRef()
    {
        _player = target != null ? target.GetComponent<Player>() : null;
    }
}
