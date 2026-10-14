using System.Collections;
using UnityEngine;

public class DirtDustEffect : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Where the attached power-up sprite appears on Phi.")]
    [SerializeField] private Transform attachPoint;

    [Tooltip("Offset from Phi's center for the attached sprite.")]
    [SerializeField] private Vector3 attachOffset = new Vector3(0f, 0.6f, 0f);

    [Tooltip("Sorting order for the attached sprite (should be above Phi).")]
    [SerializeField] private int attachedSortingOrder = 5;

    public bool IsActive { get; private set; }

    private float remainingTime;
    private float totalDuration;

    private PhiHealth phiHealth;
    private SpriteRenderer phiSprite;
    private Collider2D phiCollider;
    private Rigidbody2D phiRb;

    private GameObject attachedVisual;
    private SpriteRenderer attachedRenderer;

    private void Awake()
    {
        phiHealth = GetComponent<PhiHealth>();
        phiSprite = GetComponentInChildren<SpriteRenderer>();
        phiCollider = GetComponent<Collider2D>();
        phiRb = GetComponent<Rigidbody2D>();

        if (attachPoint == null)
            attachPoint = transform;
    }

    private void Update()
    {
        if (!IsActive) return;

        remainingTime -= Time.deltaTime;

        // Optional: blink the attached sprite to signal the timer
        if (attachedRenderer != null && remainingTime < 2f)
        {
            float blink = Mathf.PingPong(Time.time * 6f, 1f);
            attachedRenderer.enabled = blink > 0.5f;
        }

        if (remainingTime <= 0f)
            Deactivate();
    }

    public void Activate(float duration, Sprite attachedSprite)
    {
        // If already active, just extend the timer
        if (IsActive)
        {
            remainingTime = Mathf.Max(remainingTime, duration);
            return;
        }

        IsActive = true;
        totalDuration = duration;
        remainingTime = duration;

        Debug.Log($"[DirtDustEffect] Activated for {duration}s.");

        // Make Phi transparent
        if (phiSprite != null)
        {
            Color c = phiSprite.color;
            c.a = 0.25f;   // semi-transparent
            phiSprite.color = c;
        }

        // Make Phi intangible with enemies (but keep ground collision)
        SetEnemyCollisionEnabled(false);

        // Make enemies not detect Phi
        SetEnemiesIgnorePhi(true);

        // Attach the visual sprite
        if (attachedSprite != null)
            CreateAttachedVisual(attachedSprite);
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;

        Debug.Log("[DirtDustEffect] Deactivated.");

        // Restore opacity
        if (phiSprite != null)
        {
            Color c = phiSprite.color;
            c.a = 1f;
            phiSprite.color = c;
        }

        // Restore collisions
        SetEnemyCollisionEnabled(true);
        SetEnemiesIgnorePhi(false);

        // Destroy the visual indicator
        if (attachedVisual != null)
            Destroy(attachedVisual);
    }

    // ---------------------------------------------------------------
    // COLLISION HANDLING
    // ---------------------------------------------------------------
    private void SetEnemyCollisionEnabled(bool enabled)
    {
        // Use the Layer Collision Matrix to disable Player vs Enemies collisions
        // Unity's Physics2D doesn't have a per-collider disable; the cleanest way
        // is to swap Phi's layer temporarily.

        int playerLayer = LayerMask.NameToLayer("Player");
        int groundLayer = LayerMask.NameToLayer("Ground");
        int enemiesLayer = LayerMask.NameToLayer("Enemies");
        int ghostsLayer = LayerMask.NameToLayer("PlayerGhost"); // create this

        if (!enabled && ghostsLayer >= 0)
        {
            gameObject.layer = ghostsLayer;
        }
        else if (playerLayer >= 0)
        {
            gameObject.layer = playerLayer;
        }
    }

    private void SetEnemiesIgnorePhi(bool ignore)
    {
        // Disable enemy detection scripts by telling them Phi is hidden
        // We just make Phi play dead to enemy senses via tag + layer.
        //
        // If enemies check for the "Player" tag, we can temporarily change Phi's tag.
        // But tags aren't as clean to toggle. Instead, use a static flag.
        DirtDustEffect.IsPhiGhosted = ignore;
    }

    // Static flag enemies can check to skip detection
    public static bool IsPhiGhosted = false;

    // ---------------------------------------------------------------
    // VISUAL INDICATOR
    // ---------------------------------------------------------------
    private void CreateAttachedVisual(Sprite sprite)
    {
        attachedVisual = new GameObject("DirtDustVisual");
        attachedVisual.transform.SetParent(attachPoint);
        attachedVisual.transform.localPosition = attachOffset;
        attachedVisual.transform.localScale = Vector3.one;

        attachedRenderer = attachedVisual.AddComponent<SpriteRenderer>();
        attachedRenderer.sprite = sprite;
        attachedRenderer.sortingOrder = attachedSortingOrder;
    }
}