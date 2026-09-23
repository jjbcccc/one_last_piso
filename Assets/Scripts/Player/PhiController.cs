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
    }

    private void Update()
    {
        ReadInput();
        CheckGround();
        HandleJumpInput();
        HandleStance();
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
            return;
        }

        bool overlap = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);

        // Not grounded while moving upward
        if (rb.linearVelocity.y > 0.01f)
            overlap = false;

        isGrounded = overlap;

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
        if (coyoteCounter <= 0f) return;   // must be grounded or in coyote time

        Jump();
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
}