using System;
using UnityEngine;

public class PhiHealth : MonoBehaviour
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private float invincibilityDuration = 1f;

    [Header("References")]
    [SerializeField] private PhiVisuals visuals;

    private int currentLives;
    private float lastHitTime = -999f;

    public int CurrentLives => currentLives;
    public int MaxLives => maxLives;

    public event Action<int, int> OnLivesChanged;
    public event Action OnDeath;

    private void Awake()
    {
        currentLives = maxLives;
    }

    private void Start()
    {
        OnLivesChanged?.Invoke(currentLives, maxLives);
    }

    public void TakeDamage(int amount = 1)
    {
        if (currentLives <= 0) return;
        if (Time.time - lastHitTime < invincibilityDuration) return;

        currentLives -= amount;
        if (currentLives < 0) currentLives = 0;

        lastHitTime = Time.time;

        Debug.Log($"[PhiHealth] Took damage — lives left: {currentLives}");
        OnLivesChanged?.Invoke(currentLives, maxLives);

        // Blink while invincible
        if (visuals != null)
            visuals.Blink(invincibilityDuration);
        else
            Debug.LogWarning("[PhiHealth] visuals reference is not assigned — no blink.");

        if (currentLives == 0)
        {
            Debug.Log("[PhiHealth] Player died!");
            OnDeath?.Invoke();
        }
    }

    public void ResetLives()
    {
        currentLives = maxLives;
        OnLivesChanged?.Invoke(currentLives, maxLives);
    }
}