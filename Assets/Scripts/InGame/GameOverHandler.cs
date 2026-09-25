// Working GameOverHandler.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private PhiHealth phiHealth;
    [SerializeField] private GameObject gameOverPage;
    [SerializeField] private string menuSceneName = "StageSelect";

    private void Awake()
    {
        if (gameOverPage != null)
            gameOverPage.SetActive(false);
    }

    private void OnEnable()
    {
        if (phiHealth != null)
            phiHealth.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (phiHealth != null)
            phiHealth.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        Debug.Log("[GameOverHandler] Player died — showing Game Over.");

        if (gameOverPage != null)
            gameOverPage.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}