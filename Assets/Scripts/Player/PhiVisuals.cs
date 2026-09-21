using UnityEngine;

public class PhiVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PhiController phi;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [Tooltip("Sprite shown when Phi is standing (not hiding).")]
    [SerializeField] private Sprite normalSprite;

    [Tooltip("Sprite shown when Phi is hiding.")]
    [SerializeField] private Sprite hiddenSprite;

    private void Reset()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (normalSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = normalSprite;
    }

    void Update()
    {
        if (phi == null || spriteRenderer == null) return;

        Sprite desired = phi.IsHiding ? hiddenSprite : normalSprite;
        if (desired != null && spriteRenderer.sprite != desired)
            spriteRenderer.sprite = desired;
    }
}