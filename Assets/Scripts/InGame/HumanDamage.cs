using UnityEngine;

public class HumanDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private bool restartOnContact = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HandleContact(other);
    }

    private void HandleContact(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Don't hurt Phi if he's hiding
        PhiController phi = other.GetComponent<PhiController>();
        if (phi != null && phi.IsHiding) return;

        // Restart the game
        if (restartOnContact)
        {
            if (SceneTransition.Instance != null)
                SceneTransition.Instance.RestartCurrentScene();
            else
                Debug.LogWarning("SceneTransition missing — falling back to direct reload");
        }
        else
        {
            Debug.Log("Phi took damage from Human!");
            // other.GetComponent<PhiHealth>()?.TakeDamage(damageAmount);
        }
    }
}