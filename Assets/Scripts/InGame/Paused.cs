using UnityEngine;
using UnityEngine.SceneManagement;

public class Pauseed : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject pausePanel;   // the SettingsPanel (or PauseWindow)

    [Header("Scene Names")]
    [SerializeField] private string stageSelectSceneName = "StageSelect";

    public void Continue()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Unfreeze the game if you paused it
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;   // always reset before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;   // always reset before changing scene
        SceneManager.LoadScene(stageSelectSceneName);
    }

    void Update()
    {
        // Optional: toggle pause with the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf) Continue();
            else Open();
        }
    }

    public void Open()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }
}