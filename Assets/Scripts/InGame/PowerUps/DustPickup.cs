using UnityEngine;

public class DustPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Get the power-up system on the player
        DustPowerUp powerUp = other.GetComponent<DustPowerUp>();
        if (powerUp != null)
            powerUp.Activate();

        // Remove the pickup from the scene
        Destroy(gameObject);
    }
}