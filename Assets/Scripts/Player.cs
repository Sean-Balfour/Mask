using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference dashAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float deceleration = 55f;
    [SerializeField] private bool normalizeDiagonal = true;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 14f;
    [SerializeField] private float dashDuration = 0.12f;
    [SerializeField] private float dashCooldown = 0.35f;
    [SerializeField] private bool dashUsesInputOrLastDirection = true;

    [Header("Movement Meter")]
    [SerializeField] private float meterMax = 100f;
    [SerializeField] private float meterBuildRate = 35f;
    [SerializeField] private float meterDrainRate = 65f;
    [SerializeField] private float movingThreshold = 0.05f;

    [Header("Facing")]
    [SerializeField] private float rotationSpeed = 15f;

    public float Meter => _meter;
    public float MeterNormalized => meterMax <= 0f ? 0f : Mathf.Clamp01(_meter / meterMax);
    public Vector2 LastMoveDirection => _lastNonZeroDir;

    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;

    private Vector2 _rawInput;
    private Vector2 _moveDir;
    private Vector2 _lastNonZeroDir = Vector2.right;

    private float _meter;

    private bool _isDashing;
    private float _dashTimer;
    private float _dashCooldownTimer;
    private Vector2 _dashDir;

    private float _targetRotation;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();

        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
        dashAction?.action.Enable();

        if (dashAction != null)
            dashAction.action.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        if (dashAction != null)
            dashAction.action.performed -= OnDashPerformed;

        moveAction?.action.Disable();
        dashAction?.action.Disable();
    }

    private void Update()
    {
        _rawInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        _moveDir = _rawInput;

        if (normalizeDiagonal && _moveDir.sqrMagnitude > 1f)
            _moveDir.Normalize();

        if (_moveDir.sqrMagnitude > 0.0001f)
            _lastNonZeroDir = _moveDir.normalized;

        UpdateFacing();

        _dashCooldownTimer -= Time.deltaTime;

        if (_isDashing)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
                _isDashing = false;
        }

        UpdateMeter();
    }

    private void FixedUpdate()
    {
        if (_isDashing)
        {
            _rb.linearVelocity = _dashDir * dashSpeed;
            ApplyRotation();
            return;
        }

        Vector2 targetVel = _moveDir * moveSpeed;
        Vector2 vel = _rb.linearVelocity;
        float rate = targetVel.sqrMagnitude > 0.0001f ? acceleration : deceleration;

        vel = Vector2.MoveTowards(vel, targetVel, rate * Time.fixedDeltaTime);
        _rb.linearVelocity = vel;

        ApplyRotation();
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        Dash();
    }

    public void Dash()
    {
        if (_isDashing || _dashCooldownTimer > 0f)
            return;

        Vector2 chosenDir = dashUsesInputOrLastDirection
            ? (_moveDir.sqrMagnitude > 0.0001f ? _moveDir.normalized : _lastNonZeroDir.normalized)
            : _lastNonZeroDir.normalized;

        if (chosenDir.sqrMagnitude <= 0.0001f)
            chosenDir = Vector2.right;

        _dashDir = chosenDir;
        _isDashing = true;
        _dashTimer = dashDuration;
        _dashCooldownTimer = dashCooldown;

        UpdateFacing(_dashDir);
    }

    private void UpdateMeter()
    {
        float speed = _rb.linearVelocity.magnitude;

        if (speed > movingThreshold)
        {
            float intensity = Mathf.Clamp01(moveSpeed <= 0f ? 0f : speed / moveSpeed);
            _meter += meterBuildRate * intensity * Time.deltaTime;
        }
        else
        {
            _meter -= meterDrainRate * Time.deltaTime;
        }

        _meter = Mathf.Clamp(_meter, 0f, meterMax);
    }

    private void UpdateFacing()
    {
        UpdateFacing(_moveDir.sqrMagnitude > 0.0001f ? _moveDir : _lastNonZeroDir);
    }

    private void UpdateFacing(Vector2 dir)
    {
        if (dir.sqrMagnitude <= 0.0001f)
            return;

        bool flip = dir.x < 0f;
        if (_sprite != null) _sprite.flipX = flip;

        float angle = Mathf.Atan2(dir.y, Mathf.Abs(dir.x)) * Mathf.Rad2Deg;

        if (flip)
            angle = -angle;

        _targetRotation = Mathf.Clamp(angle, -90f, 90f);
    }

    private void ApplyRotation()
    {
        float newRotation = Mathf.LerpAngle(
            _rb.rotation,
            _targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        _rb.rotation = newRotation;
    }
}
