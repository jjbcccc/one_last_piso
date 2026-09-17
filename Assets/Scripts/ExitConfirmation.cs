using UnityEngine;

public class ExitConfirmation : MonoBehaviour
{
    [Header("Exit Confirmation Panel")]
    public GameObject exitConfirmationPanel;

    public void OpenExitConfirmation()
    {
        exitConfirmationPanel.SetActive(true);
    }

    public void CancelExit()
    {
        exitConfirmationPanel.SetActive(false);
    }

    public void ConfirmExit()
    {
        Application.Quit();

        // This allows you to see the result while testing
        // inside the Unity Editor.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}