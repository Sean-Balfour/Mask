using Unity.Android.Types;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof (Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float maxSpeed = 5.0f;
    [SerializeField]
    private float acceleration = 18.0f;
    [SerializeField]
    private float deceleration = 10.0f;

    [SerializeField]
    private float dashSpeed = 12.0f;
    [SerializeField]
    private float dashDuration = 0.18f;
    [SerializeField]
    private float dashCooldown = 0.6f;

    private Rigidbody2D rigidbody;
    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.right;

    private float dashTimer;
    private float dashCoooldownTimer;
    private bool isDashing;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0.0f;
        rigidbody.linearDamping = 0.0f;
    }

    private void Update()
    {
        ReadInput();
        ReadDashInput();
    }

    private void FixedUpdate()
    {
        UpdateDashTimers();

        if (isDashing)
            return;

        ApplyMovement();
        ClampSpeed();
    }

    private void ReadInput()
    {
        Vector2 move = Vector2.zero;

        if (Gamepad.current != null)
        {
            move = Gamepad.current.leftStick.ReadValue();
        }
        else if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                move.y += 1;
            if (Keyboard.current.sKey.isPressed)
                move.y -= 1;
            if (Keyboard.current.aKey.isPressed)
                move.x -= 1;
            if (Keyboard.current.dKey.isPressed)
                move.x += 1;
        }

        moveInput = move;

        if (moveInput.sqrMagnitude > 0.001f)
            lastMoveDir = moveInput.normalized;
    }

    private void ReadDashInput()
    {
        if (dashCoooldownTimer > 0.0f || isDashing)
            return;

        bool dashPressed = (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) || (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);

        if (!dashPressed)
            return;

        Vector2 dashDirection = moveInput.sqrMagnitude > 0.001f ? moveInput.normalized : lastMoveDir;

        StartDash(dashDirection);
    }

    private void StartDash(Vector2 dashDirection)
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCoooldownTimer = dashCooldown;

        rigidbody.linearVelocity = dashDirection * dashSpeed;
    }

    private void UpdateDashTimers()
    {
        if (dashCoooldownTimer > 0.0f)
            dashCoooldownTimer -= Time.fixedDeltaTime;

        if (!isDashing)
            return;

        dashTimer -= Time.fixedDeltaTime;


        if (dashTimer <= 0.0f)
        {
            isDashing = false;
        }
    }

    private void ApplyMovement()
    {
        if (moveInput.sqrMagnitude > 0.0001f)
        {
            rigidbody.AddForce(moveInput * acceleration, ForceMode2D.Force);
        }
        else
        {
            // Smooth water slowdown
            Vector2 v = rigidbody.linearVelocity;

            if (v.magnitude < 0.05f)
            {
                rigidbody.linearVelocity = Vector2.zero;
            }
            else
            {
                rigidbody.AddForce(-v.normalized * deceleration, ForceMode2D.Force);
            }
        }
    }

    private void ClampSpeed()
    {
        Vector2 v = rigidbody.linearVelocity;

        if (v.magnitude > maxSpeed)
            rigidbody.linearVelocity = v / v.magnitude * maxSpeed;
    }
}
