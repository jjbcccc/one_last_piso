using UnityEngine;
using UnityEngine.InputSystem;

public class PhiController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 35f;
    [SerializeField] private float deceleration = 45f;
    [SerializeField] private float airAcceleration = 25f;
    [SerializeField] private float airDeceleration = 20f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float groundCheckLockout = 0.1f;
    [SerializeField] private bool debugGround = false;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Stance")]
    [SerializeField] private float hiddenScaleY = 0.45f;

    [Header("Safe Position Tracking")]
    [SerializeField] private float safePositionRecordInterval = 0.05f;   // how often to record

    private Vector2 lastSafePosition;
    private float safeRecordTimer;

    private Rigidbody2D rb;

    private float horizontalInput;

    private float coyoteCounter;
    private float jumpBufferCounter;
    private int jumpsRemaining;
    private float lockoutCounter;

    private bool isGrounded;
    private bool isHiding;

    private Vector3 normalScale;

    public bool IsHiding => isHiding;

    // External input from mobile UI buttons
    public float ExternalHorizontal { get; set; }
    public bool ExternalJumpPressed { get; set; }
    public bool ExternalHidePressed { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        normalScale = transform.localScale;
        jumpsRemaining = maxJumps;

        // Initialize safe position to spawn position
        lastSafePosition = transform.position;
    }

    private void Update()
    {
        ReadInput();
        CheckGround();
        HandleJumpInput();
        HandleStance();
        TrackSafePosition();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void ReadInput()
    {
        if (isHiding)
        {
            horizontalInput = 0f;
            return;
        }

        // --- Keyboard via new Input System ---
        float keyboard = 0f;
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) keyboard += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) keyboard -= 1f;
        }

        // Keyboard takes priority; otherwise use touch input
        horizontalInput = Mathf.Abs(keyboard) > 0.01f ? keyboard : ExternalHorizontal;
    }

    private void CheckGround()
    {
        if (lockoutCounter > 0f)
        {
            lockoutCounter -= Time.deltaTime;
            isGrounded = false;
            coyoteCounter -= Time.deltaTime;
            if (debugGround) Debug.Log($"[locked out] velY={rb.linearVelocity.y:F2}");
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            jumpsRemaining = maxJumps;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (debugGround)
            Debug.Log($"grounded={isGrounded} velY={rb.linearVelocity.y:F2} jumps={jumpsRemaining}");
    }

    private void HandleMovement()
    {
        float targetSpeed = isHiding ? 0f : horizontalInput * moveSpeed;
        float difference = targetSpeed - rb.linearVelocity.x;

        float currentAcceleration;

        if (isHiding)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            currentAcceleration = isGrounded ? acceleration : airAcceleration;
        }
        else
        {
            currentAcceleration = isGrounded ? deceleration : airDeceleration;
        }

        float movement = difference * currentAcceleration;

        rb.AddForce(Vector2.right * movement, ForceMode2D.Force);

        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -moveSpeed, moveSpeed);
        rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);
    }

    private void HandleJumpInput()
    {
        bool keyboardJump = false;
        var kb = Keyboard.current;
        if (kb != null)
        {
            keyboardJump = kb.spaceKey.wasPressedThisFrame
                        || kb.upArrowKey.wasPressedThisFrame;
        }

        bool jumpDown = keyboardJump || ExternalJumpPressed;
        ExternalJumpPressed = false;

        if (jumpDown)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (isHiding)
        {
            jumpBufferCounter = 0f;
            return;
        }

        if (jumpBufferCounter <= 0f) return;
        if (jumpsRemaining <= 0) return;

        bool isFirstJump = jumpsRemaining == maxJumps;

        // Only the first jump needs to be grounded or within coyote time.
        // The second (double) jump only needs an available jump charge.
        if (isFirstJump && coyoteCounter <= 0f) return;

        Jump(!isFirstJump);
        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
        jumpsRemaining--;
    }

    private void Jump(bool isDoubleJump = false)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        lockoutCounter = groundCheckLockout;
    }

    private void HandleStance()
    {
        // --- Keyboard hide toggle via new Input System ---
        bool keyboardHide = false;
        var kb = Keyboard.current;
        if (kb != null)
        {
            keyboardHide = kb.sKey.wasPressedThisFrame
                        || kb.downArrowKey.wasPressedThisFrame;
        }

        bool hideDown = keyboardHide || ExternalHidePressed;
        ExternalHidePressed = false;

        if (hideDown && (isGrounded || isHiding))
        {
            SetHiding(!isHiding);
        }
    }

    private void SetHiding(bool hiding)
    {
        isHiding = hiding;

        if (isHiding)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else
        {
            transform.localScale = normalScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void TrackSafePosition()
    {
        safeRecordTimer -= Time.deltaTime;

        if (safeRecordTimer > 0f) return;

        safeRecordTimer = safePositionRecordInterval;

        // Only record if Phi is firmly on the ground and standing still vertically
        if (!isGrounded) return;
        if (rb.linearVelocity.y > 0.5f) return;   // not while jumping

        // Record the current position as the last safe spot
        lastSafePosition = transform.position;
    }

    public void ReturnToSafePosition()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        transform.position = new Vector3(
            lastSafePosition.x,
            lastSafePosition.y,
            transform.position.z
        );

        Debug.Log($"[PhiController] Returned to safe position: {lastSafePosition}");
    }
}