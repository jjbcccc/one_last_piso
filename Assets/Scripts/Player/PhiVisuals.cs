using System.Collections;
using UnityEngine;

public class PhiVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PhiController phi;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hiddenSprite;

    [Header("Blink")]
    [SerializeField] private float blinkInterval = 0.08f;

    private Coroutine blinkRoutine;

    private void Reset()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
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
        if (blinkRoutine != null) return;   // don't override blink

        Sprite desired = phi.IsHiding ? hiddenSprite : normalSprite;
        if (desired != null && spriteRenderer.sprite != desired)
            spriteRenderer.sprite = desired;
    }

    public void Blink(float duration)
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(BlinkRoutine(duration));
    }

    private IEnumerator BlinkRoutine(float duration)
    {
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < duration)
        {
            visible = !visible;
            spriteRenderer.enabled = visible;

            yield return new WaitForSecondsRealtime(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true;
        blinkRoutine = null;
    }
}