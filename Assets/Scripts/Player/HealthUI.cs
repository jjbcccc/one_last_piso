using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PhiHealth phiHealth;

    [Header("Hearts")]
    [SerializeField] private Image[] hearts;         // assign in Inspector

    [Header("Colors")]
    [SerializeField] private Color fullColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.25f);

    private void OnEnable()
    {
        if (phiHealth == null) return;
        phiHealth.OnLivesChanged += UpdateHearts;
        // Sync immediately in case PhiHealth already fired OnLivesChanged
        UpdateHearts(phiHealth.CurrentLives, phiHealth.MaxLives);
    }

    private void OnDisable()
    {
        if (phiHealth == null) return;
        phiHealth.OnLivesChanged -= UpdateHearts;
    }

    private void UpdateHearts(int currentLives, int maxLives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;
            hearts[i].color = (i < currentLives) ? fullColor : emptyColor;
        }
    }
}