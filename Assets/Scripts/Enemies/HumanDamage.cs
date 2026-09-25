using UnityEngine;

public class HumanDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D other) => TryDamage(other);
    private void OnTriggerStay2D(Collider2D other) => TryDamage(other);

    private void TryDamage(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Don't hurt Phi if he's hiding
        PhiController phi = other.GetComponent<PhiController>();
        if (phi != null && phi.IsHiding) return;

        // Apply damage — PhiHealth handles invincibility + blink internally
        PhiHealth health = other.GetComponent<PhiHealth>();
        if (health != null)
            health.TakeDamage(damageAmount);
    }
}