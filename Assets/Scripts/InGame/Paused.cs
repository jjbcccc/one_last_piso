using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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
        // Toggle pause with the Escape key (new Input System)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pausePanel != null && pausePanel.activeSelf)
                Continue();
            else
                Open();
        }
    }

    public void Open()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }
}