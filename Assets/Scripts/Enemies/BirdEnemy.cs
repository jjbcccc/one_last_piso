using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    public enum State { Patrol, Chase }
    public enum DetectionType { None, Sight }

    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 1.8f;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float patrolPauseDuration = 0.6f;

    [Header("Patrol Station")]
    [SerializeField] private float maxChaseDistance = 8f;
    [SerializeField] private float maxVerticalChase = 3f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private float chaseGiveUpTime = 2.5f;
    [SerializeField] private Transform chaseTarget;

    [Header("Detection — Sight Only")]
    [SerializeField] private float sightRange = 12f;
    [SerializeField] private float sightAngle = 90f;
    [SerializeField] private LayerMask sightBlockers;

    [Header("References")]
    [SerializeField] private Transform eyePoint;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprite Default Facing")]
    [Tooltip("Check this if your sprite is drawn facing LEFT by default.")]
    [SerializeField] private bool spriteFacesLeftByDefault = false;

    [Header("Bobbing Animation")]
    [SerializeField] private float bobAmplitude = 0.15f;
    [SerializeField] private float bobSpeed = 2f;

    [Header("Debug")]
    [SerializeField] private bool debugSight = false;

    private Rigidbody2D rb;

    private State state = State.Patrol;
    private DetectionType lastDetection = DetectionType.None;

    private Vector2 patrolCenter;

    // patrolDirection drives the horizontal patrol movement
    private int patrolDirection = 1;

    // facingDirection drives the sight cone and sprite flip
    // It starts the same as patrolDirection but updates independently.
    private int facingDirection = 1;

    private float pauseTimer;
    private float lastSeenTimer;
    private float bobTimer;

    private PhiController phi;

    public State CurrentState => state;
    public DetectionType LastDetection => lastDetection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        patrolCenter = transform.position;

        if (eyePoint == null)
            eyePoint = transform;

        if (chaseTarget != null)
            phi = chaseTarget.GetComponent<PhiController>();

        patrolDirection = 1;
        facingDirection = 1;
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

        // Determine which way the sprite should face
        bool facingRight = facingDirection > 0;

        // If the sprite is drawn facing left by default, flipX should be
        // the opposite of the facing right logic.
        if (spriteFacesLeftByDefault)
            spriteRenderer.flipX = facingRight;
        else
            spriteRenderer.flipX = !facingRight;
    }

    // ---------------------------------------------------------------------
    // PATROL
    // ---------------------------------------------------------------------
    private void DoPatrol()
    {
        // Keep facing synced with patrol movement
        facingDirection = patrolDirection;

        bobTimer += Time.fixedDeltaTime;
        float bobOffset = Mathf.Sin(bobTimer * bobSpeed) * bobAmplitude;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            transform.position = new Vector3(
                transform.position.x,
                patrolCenter.y + bobOffset,
                transform.position.z
            );
            DetectPlayer();
            return;
        }

        rb.linearVelocity = new Vector2(patrolDirection * patrolSpeed, 0f);

        float distFromCenter = transform.position.x - patrolCenter.x;

        if (patrolDirection > 0 && distFromCenter >= patrolDistance)
        {
            patrolDirection = -1;
            facingDirection = -1;   // update immediately so sight flips
            pauseTimer = patrolPauseDuration;
        }
        else if (patrolDirection < 0 && distFromCenter <= -patrolDistance)
        {
            patrolDirection = 1;
            facingDirection = 1;
            pauseTimer = patrolPauseDuration;
        }

        DetectPlayer();
    }

    // ---------------------------------------------------------------------
    // CHASE — moves diagonally toward Phi
    // ---------------------------------------------------------------------
    private void DoChase()
    {
        if (chaseTarget == null)
        {
            ReturnToPatrol();
            return;
        }

        Vector2 targetPos = chaseTarget.position;
        float clampedTargetX = Mathf.Clamp(
            targetPos.x,
            patrolCenter.x - maxChaseDistance,
            patrolCenter.x + maxChaseDistance
        );
        float clampedTargetY = Mathf.Clamp(
            targetPos.y,
            patrolCenter.y - maxVerticalChase,
            patrolCenter.y + maxVerticalChase
        );

        Vector2 clampedTarget = new Vector2(clampedTargetX, clampedTargetY);
        Vector2 toTarget = clampedTarget - (Vector2)transform.position;
        float distToTarget = toTarget.magnitude;

        if (distToTarget < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            Vector2 dir = toTarget / distToTarget;

            Vector2 nextPos = (Vector2)transform.position + dir * chaseSpeed * Time.fixedDeltaTime;

            nextPos.x = Mathf.Clamp(
                nextPos.x,
                patrolCenter.x - maxChaseDistance,
                patrolCenter.x + maxChaseDistance
            );
            nextPos.y = Mathf.Clamp(
                nextPos.y,
                patrolCenter.y - maxVerticalChase,
                patrolCenter.y + maxVerticalChase
            );

            Vector2 clampedVel = (nextPos - (Vector2)transform.position) / Time.fixedDeltaTime;
            rb.linearVelocity = clampedVel;
        }

        // During chase, face the target horizontally
        if (Mathf.Abs(toTarget.x) > 0.05f)
            facingDirection = toTarget.x > 0 ? 1 : -1;

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
        pauseTimer = patrolPauseDuration;
        bobTimer = 0f;
    }

    // ---------------------------------------------------------------------
    // DETECTION — SIGHT ONLY
    // ---------------------------------------------------------------------
    private DetectionType DetectPlayer()
    {
        if (chaseTarget == null) return DetectionType.None;

        if (phi != null && phi.IsHiding)
        {
            lastDetection = DetectionType.None;
            return DetectionType.None;
        }

        Vector2 toTarget = (Vector2)chaseTarget.position - (Vector2)eyePoint.position;
        float distance = toTarget.magnitude;

        if (distance > sightRange)
        {
            if (debugSight) Debug.Log($"[Bird] Out of sight range ({distance:F1} > {sightRange})");
            lastDetection = DetectionType.None;
            return DetectionType.None;
        }

        // Use facingDirection for the sight cone
        Vector2 facingDir = new Vector2(facingDirection, 0f);
        float angle = Vector2.Angle(facingDir, toTarget);

        if (angle > sightAngle * 0.5f)
        {
            if (debugSight)
                Debug.Log($"[Bird] Phi outside cone (angle {angle:F1}° > {sightAngle * 0.5f}°), facing dir {facingDirection}");
            lastDetection = DetectionType.None;
            return DetectionType.None;
        }

        RaycastHit2D hit = Physics2D.Linecast(
            eyePoint.position,
            chaseTarget.position,
            sightBlockers
        );

        if (hit.collider != null)
        {
            if (debugSight) Debug.Log($"[Bird] Line of sight blocked by {hit.collider.name}");
            lastDetection = DetectionType.None;
            return DetectionType.None;
        }

        if (debugSight) Debug.Log("[Bird] SPOTTED Phi!");
        EnterChase(DetectionType.Sight);
        return DetectionType.Sight;
    }

    private void EnterChase(DetectionType type)
    {
        lastDetection = type;
        if (state != State.Chase)
        {
            state = State.Chase;
            lastSeenTimer = 0f;
            Debug.Log("[BirdEnemy] Spotted Phi — diving!");
        }
    }

    // ---------------------------------------------------------------------
    // GIZMOS
    // ---------------------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        Vector3 eye = eyePoint != null ? eyePoint.position : transform.position;
        Vector3 center = Application.isPlaying ? (Vector3)patrolCenter : transform.position;

        // Sight cone — drawn facing the CURRENT facing direction
        int drawDir = Application.isPlaying ? facingDirection : 1;

        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Vector3 forward = new Vector3(drawDir, 0f, 0f);
        Vector3 leftDir = Quaternion.Euler(0, 0, sightAngle * 0.5f) * forward;
        Vector3 rightDir = Quaternion.Euler(0, 0, -sightAngle * 0.5f) * forward;

        Gizmos.DrawRay(eye, leftDir * sightRange);
        Gizmos.DrawRay(eye, rightDir * sightRange);
        Gizmos.DrawWireSphere(eye, sightRange);

        // Highlight the center ray (shows the facing direction)
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(eye, forward * sightRange);

        // Patrol path
        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            new Vector3(center.x - patrolDistance, center.y, 0f),
            new Vector3(center.x + patrolDistance, center.y, 0f)
        );

        // Chase bounds
        Gizmos.color = Color.red;
        float left = center.x - maxChaseDistance;
        float right = center.x + maxChaseDistance;
        float bottom = center.y - maxVerticalChase;
        float top = center.y + maxVerticalChase;

        Gizmos.DrawLine(new Vector3(left, bottom), new Vector3(right, bottom));
        Gizmos.DrawLine(new Vector3(right, bottom), new Vector3(right, top));
        Gizmos.DrawLine(new Vector3(right, top), new Vector3(left, top));
        Gizmos.DrawLine(new Vector3(left, top), new Vector3(left, bottom));
    }
}