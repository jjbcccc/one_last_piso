using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    [Header("Effect")]
    [Tooltip("Duration in seconds for the power-up effect.")]
    [SerializeField] private float duration = 6f;

    [Header("Visual")]
    [Tooltip("Sprite that sticks to Phi after pickup (e.g., dirt/dust icon).")]
    [SerializeField] private Sprite attachedSprite;

    [Header("Behavior")]
    [Tooltip("If true, the pickup is destroyed after collection.")]
    [SerializeField] private bool destroyOnPickup = true;

    [Tooltip("If true, the pickup can be collected again after Phi loses the effect.")]
    [SerializeField] private bool respawns = false;

    [SerializeField] private float respawnDelay = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Don't collect if Phi is already under this effect
        DirtDustEffect effect = other.GetComponent<DirtDustEffect>();
        if (effect != null && effect.IsActive) return;

        // Apply the effect
        if (effect == null)
            effect = other.gameObject.AddComponent<DirtDustEffect>();

        effect.Activate(duration, attachedSprite);

        Debug.Log($"[PowerUpPickup] {name} collected — {duration}s of effect.");

        if (respawns)
            StartCoroutine(RespawnAfterDelay());
        else if (destroyOnPickup)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator RespawnAfterDelay()
    {
        // Hide the pickup
        gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        gameObject.SetActive(true);
    }
}