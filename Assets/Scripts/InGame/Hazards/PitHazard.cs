using UnityEngine;

public class PitHazard : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damageAmount = 1;

    [Header("Behavior")]
    [Tooltip("If true, Phi blinks and can't be hit again for the invincibility duration.")]
    [SerializeField] private bool respectInvincibility = true;

    private void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    private void OnTriggerStay2D(Collider2D other) => TryHit(other);

    private void TryHit(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PhiHealth health = other.GetComponent<PhiHealth>();
        PhiController controller = other.GetComponent<PhiController>();

        if (health == null || controller == null) return;

        // Apply damage (PhiHealth already handles invincibility internally)
        health.TakeDamage(damageAmount);

        // Revert to last safe position
        controller.ReturnToSafePosition();

        Debug.Log("[PitHazard] Phi touched the pit — damage + revert.");
    }
}