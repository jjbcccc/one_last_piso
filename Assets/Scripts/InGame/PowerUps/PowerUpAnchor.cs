using UnityEngine;

/// Attaches an icon above Phi's head that never rotates with him.
public class PowerUpAnchor : MonoBehaviour
{
    [SerializeField] private Transform followTarget;   // the Visual (or Phi)
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 1f, 0f);

    private void LateUpdate()
    {
        if (followTarget == null) return;

        // Position: world position of target + offset in world space (ignoring rotation)
        transform.position = followTarget.position + localOffset;

        // Rotation: always upright
        transform.rotation = Quaternion.identity;
    }
}