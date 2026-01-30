using UnityEngine;

/// <summary>
/// 2D camera follow with adjustable trailing/look-ahead + level bounds clamping.
/// Attach to an orthographic Camera.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    public enum OffsetMode
    {
        None,
        TrailBehind,
        LookAhead
    }

    [Header("Target")]
    [SerializeField] private Transform target;

    [Tooltip("Camera offset relative to the target (world units).")]
    [SerializeField] private Vector2 offset = Vector2.zero;

    [Header("Follow Feel")]
    [Tooltip("How quickly the camera catches up. Higher = snappier, lower = more floaty.")]
    [SerializeField] private float followSharpness = 8f;

    [Header("Directional Offset")]
    [SerializeField] private OffsetMode mode = OffsetMode.TrailBehind;

    [Tooltip("How far to offset the camera in the movement direction (or opposite for trailing).")]
    [SerializeField] private float directionalDistance = 1.5f;

    [Tooltip("How quickly the movement direction updates (prevents jitter). Higher = more responsive.")]
    [SerializeField] private float directionSharpness = 10f;

    [Tooltip("Minimum target movement speed before directional offset kicks in (world units/sec).")]
    [SerializeField] private float minSpeedForDirection = 0.05f;

    [Header("Bounds")]
    [Tooltip("If assigned, camera is clamped to this BoxCollider2D's bounds (recommended).")]
    [SerializeField] private BoxCollider2D boundsCollider;

    [Tooltip("Extra padding inside bounds (world units). Positive shrinks usable camera area.")]
    [SerializeField] private Vector2 boundsPadding = Vector2.zero;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = false;

    private Camera cam;
    private Vector2 lastTargetPos;
    private Vector2 smoothedMoveDir = Vector2.right;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (target != null)
            lastTargetPos = target.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Estimate velocity from position delta (works even if target doesn't have Rigidbody2D)
        Vector2 targetPos = target.position;
        Vector2 delta = targetPos - lastTargetPos;

        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        Vector2 targetVel = delta / dt;

        // Smooth movement direction for stable trailing/look-ahead
        if (targetVel.magnitude > minSpeedForDirection)
        {
            Vector2 desiredDir = targetVel.normalized;
            float dirLerp = 1f - Mathf.Exp(-directionSharpness * Time.deltaTime);
            smoothedMoveDir = Vector2.Lerp(smoothedMoveDir, desiredDir, dirLerp).normalized;
        }

        lastTargetPos = targetPos;

        // Compute directional offset depending on mode
        Vector2 directionalOffset = Vector2.zero;

        switch (mode)
        {
            case OffsetMode.TrailBehind:
                directionalOffset = -smoothedMoveDir * directionalDistance;
                break;

            case OffsetMode.LookAhead:
                directionalOffset = smoothedMoveDir * directionalDistance;
                break;

            case OffsetMode.None:
            default:
                directionalOffset = Vector2.zero;
                break;
        }

        Vector2 desired = targetPos + offset + directionalOffset;

        // Smooth follow (exponential smoothing)
        Vector2 current = transform.position;
        float followLerp = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);
        Vector2 next = Vector2.Lerp(current, desired, followLerp);

        // Clamp to bounds (if any)
        if (boundsCollider != null)
            next = ClampToBounds(next);

        transform.position = new Vector3(next.x, next.y, transform.position.z);
    }

    private Vector2 ClampToBounds(Vector2 desiredPos)
    {
        Bounds b = boundsCollider.bounds;

        float halfH = cam.orthographicSize;
        float halfW = cam.orthographicSize * cam.aspect;

        float minX = b.min.x + halfW + boundsPadding.x;
        float maxX = b.max.x - halfW - boundsPadding.x;
        float minY = b.min.y + halfH + boundsPadding.y;
        float maxY = b.max.y - halfH - boundsPadding.y;

        // If bounds smaller than camera view, center on bounds axis
        if (minX > maxX) desiredPos.x = (b.min.x + b.max.x) * 0.5f;
        else desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);

        if (minY > maxY) desiredPos.y = (b.min.y + b.max.y) * 0.5f;
        else desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);

        return desiredPos;
    }

    public void SetTarget(Transform newTarget, bool snapInstantly = true)
    {
        target = newTarget;
        if (target == null) return;

        lastTargetPos = target.position;

        if (snapInstantly)
        {
            Vector2 pos = target.position;
            Vector2 desired = pos + offset;
            if (boundsCollider != null) desired = ClampToBounds(desired);
            transform.position = new Vector3(desired.x, desired.y, transform.position.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        if (boundsCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(boundsCollider.bounds.center, boundsCollider.bounds.size);
        }

        if (target != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 t = target.position;
            Gizmos.DrawLine(t, t + (Vector3)(smoothedMoveDir * 2f));
        }
    }
}
