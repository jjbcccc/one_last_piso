using UnityEngine;

public class FallingHazard : MonoBehaviour
{
    [Header("Falling Settings")]
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Damage")]
    [SerializeField] private int damageAmount = 1;
    [Tooltip("If true, hiding protects Phi from this hazard.")]
    [SerializeField] private bool hideProtectsPhi = false;

    [Header("Lifetime")]
    [SerializeField] private float destroyAfterSeconds = 3f;
    [SerializeField] private bool destroyOnHit = false;
    [SerializeField] private bool destroyOnGroundContact = false;

    private void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    private void Update()
    {
        transform.Translate(
            Vector3.down * fallSpeed * Time.deltaTime,
            Space.World
        );

        transform.Rotate(
            Vector3.forward * rotationSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Optional hide protection
        if (hideProtectsPhi)
        {
            PhiController phi = other.GetComponent<PhiController>();
            if (phi != null && phi.IsHiding) return;
        }

        PhiHealth health = other.GetComponent<PhiHealth>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
            Debug.Log($"[FallingHazard] Hit Phi for {damageAmount}");
        }

        if (destroyOnHit)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!destroyOnGroundContact) return;
        if (collision.collider.CompareTag("Player")) return;

        Destroy(gameObject);
    }
}