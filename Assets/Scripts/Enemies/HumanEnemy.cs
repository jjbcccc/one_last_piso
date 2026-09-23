using UnityEngine;

public class HumanEnemy : MonoBehaviour
{
    public enum State { Patrol, Chase }
    public enum DetectionType { None, Hearing, Sight }

    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float patrolPauseDuration = 0.5f;

    [Header("Patrol Boundaries")]
    [SerializeField] private float patrolOriginX;
    [SerializeField] private float maxChaseDistance = 6f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseGiveUpTime = 2f;   // seconds without detection before returning to patrol
    [SerializeField] private Transform chaseTarget;        // Phi (assigned in Inspector)

    [Header("Detection — Sight")]
    [SerializeField] private float sightRange = 8f;
    [SerializeField] private float sightAngle = 60f;       // degrees, in front of the enemy
    [SerializeField] private LayerMask sightBlockers;      // walls/ground layers that block vision

    [Header("Detection — Hearing")]
    [SerializeField] private float hearingRange = 4f;

    [Header("References")]
    [SerializeField] private Transform eyePoint;           // optional, defaults to transform
    [SerializeField] private Transform groundCheckLeft;
    [SerializeField] private Transform groundCheckRight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.15f;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;

    private State state = State.Patrol;
    private DetectionType lastDetection = DetectionType.None;

    private Vector2 startPos;
    private int direction = 1;
    private float pauseTimer;
    private float lastSeenTimer;

    private PhiController phi;

    private Vector2 originPosition;
    private Vector2 patrolCenter;

    // Public for debugging / UI
    public State CurrentState => state;
    public DetectionType LastDetection => lastDetection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        originPosition = transform.position;
        patrolCenter = originPosition;

        if (eyePoint == null)
            eyePoint = transform;

        if (chaseTarget != null)
            phi = chaseTarget.GetComponent<PhiController>();
    }

    private void FixedUpdate()
    {
        UpdateSpriteDirection();
        switch (state)
        {
            case State.Patrol: DoPatrol(); break;
            case State.Chase: DoChase(); break;
        }
    }

    private void UpdateSpriteDirection()
    {
        if (spriteRenderer == null) return;
        // direction = 1 means moving right → don't flip (sprite faces right by default)
        // direction = -1 means moving left  → flip
        spriteRenderer.flipX = (direction < 0);
    }

    // ---------------------------------------------------------------------
    // PATROL
    // ---------------------------------------------------------------------
    private void DoPatrol()
    {
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            DetectPlayer();
            return;
        }

        rb.linearVelocity = new Vector2(direction * patrolSpeed, rb.linearVelocity.y);

        float distFromCenter = transform.position.x - patrolCenter.x;

        if (direction > 0 && distFromCenter >= patrolDistance)
        {
            direction = -1;
            pauseTimer = patrolPauseDuration;
        }
        else if (direction < 0 && distFromCenter <= -patrolDistance)
        {
            direction = 1;
            pauseTimer = patrolPauseDuration;
        }

        if (IsAboutToFall())
        {
            direction *= -1;
            pauseTimer = patrolPauseDuration;
        }

        DetectPlayer();
    }

    private bool IsAboutToFall()
    {
        // Look ahead of the walk direction
        Transform check = direction > 0 ? groundCheckRight : groundCheckLeft;
        if (check == null) return false;

        bool groundAhead = Physics2D.OverlapCircle(check.position, groundCheckRadius, groundLayer);
        return !groundAhead;
    }

    // ---------------------------------------------------------------------
    // CHASE
    // ---------------------------------------------------------------------
    private void DoChase()
    {
        if (chaseTarget == null)
        {
            ReturnToPatrol();
            return;
        }

        // --- Clamp the effective chase target into our patrol range ---
        float clampedTargetX = Mathf.Clamp(
            chaseTarget.position.x,
            patrolCenter.x - maxChaseDistance,
            patrolCenter.x + maxChaseDistance
        );

        // --- Determine direction toward the (clamped) target ---
        float dirToTarget = Mathf.Sign(clampedTargetX - transform.position.x);

        // If we're already essentially at the target, stop
        if (Mathf.Abs(clampedTargetX - transform.position.x) < 0.05f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else
        {
            // --- Predict next frame's X and clamp it ---
            float nextX = transform.position.x
                        + dirToTarget * chaseSpeed * Time.fixedDeltaTime;

            // Hard clamp — enemy physically cannot cross this line
            nextX = Mathf.Clamp(
                nextX,
                patrolCenter.x - maxChaseDistance,
                patrolCenter.x + maxChaseDistance
            );

            // Convert the clamped position back into velocity for this frame
            float clampedVelocityX = (nextX - transform.position.x) / Time.fixedDeltaTime;

            rb.linearVelocity = new Vector2(clampedVelocityX, rb.linearVelocity.y);
        }

        if (dirToTarget != 0)
            direction = (int)Mathf.Sign(dirToTarget);

        // --- Detection & give-up timer ---
        DetectionType detection = DetectPlayer();
        if (detection != DetectionType.None)
            lastSeenTimer = 0f;
        else
            lastSeenTimer += Time.fixedDeltaTime;

        if (lastSeenTimer >= chaseGiveUpTime)
            ReturnToPatrol();
    }

    private void ReturnToPatrol()
    {
        state = State.Patrol;
        // Walk back toward patrol origin (optional: could just resume)
        //startPos = transform.position;
        pauseTimer = patrolPauseDuration;
    }

    // ---------------------------------------------------------------------
    // DETECTION
    // ---------------------------------------------------------------------
    private DetectionType DetectPlayer()
    {
        if (chaseTarget == null) return DetectionType.None;

        // If Phi is hiding, he's invisible AND silent
        if (phi != null && phi.IsHiding)
        {
            lastDetection = DetectionType.None;
            return DetectionType.None;
        }

        // --- Sight check ---
        Vector2 toTarget = (Vector2)chaseTarget.position - (Vector2)eyePoint.position;
        float distance = toTarget.magnitude;

        if (distance <= sightRange)
        {
            Vector2 facingDir = new Vector2(direction, 0f);
            float angle = Vector2.Angle(facingDir, toTarget);

            if (angle <= sightAngle * 0.5f)
            {
                // Is the line of sight blocked by a wall?
                RaycastHit2D hit = Physics2D.Linecast(
                    eyePoint.position,
                    chaseTarget.position,
                    sightBlockers
                );

                if (hit.collider == null)
                {
                    EnterChase(DetectionType.Sight);
                    return DetectionType.Sight;
                }
            }
        }

        // --- Hearing check ---
        if (distance <= hearingRange)
        {
            // Optional: only "hear" Phi if he's moving
            bool phiMoving = false;
            if (phi != null)
            {
                Rigidbody2D phiRb = phi.GetComponent<Rigidbody2D>();
                if (phiRb != null)
                    phiMoving = Mathf.Abs(phiRb.linearVelocity.x) > 0.1f;
            }

            if (phiMoving)
            {
                EnterChase(DetectionType.Hearing);
                return DetectionType.Hearing;
            }
        }

        lastDetection = DetectionType.None;
        return DetectionType.None;
    }

    private void EnterChase(DetectionType type)
    {
        lastDetection = type;
        if (state != State.Chase)
        {
            state = State.Chase;
            lastSeenTimer = 0f;
            Debug.Log($"[HumanEnemy] Detected via {type} — chasing!");
        }
    }

    // ---------------------------------------------------------------------
    // GIZMOS — visual debug
    // ---------------------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        Vector3 eye = eyePoint != null ? eyePoint.position : transform.position;

        // Sight range (arc)
        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Vector3 leftDir = Quaternion.Euler(0, 0, sightAngle * 0.5f) * Vector3.right;
        Vector3 rightDir = Quaternion.Euler(0, 0, -sightAngle * 0.5f) * Vector3.right;
        Gizmos.DrawRay(eye, leftDir * sightRange);
        Gizmos.DrawRay(eye, rightDir * sightRange);
        Gizmos.DrawWireSphere(eye, sightRange);

        // Hearing range
        Gizmos.color = new Color(0f, 0.6f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        // Patrol boundaries
        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            new Vector3(startPos.x - patrolDistance, transform.position.y - 0.5f, 0f),
            new Vector3(startPos.x - patrolDistance, transform.position.y + 0.5f, 0f)
        );
        Gizmos.DrawLine(
            new Vector3(startPos.x + patrolDistance, transform.position.y - 0.5f, 0f),
            new Vector3(startPos.x + patrolDistance, transform.position.y + 0.5f, 0f)
        );
    }
}