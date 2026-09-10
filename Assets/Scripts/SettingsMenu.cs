using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    void Start()
    {
        Debug.Log("[SettingsMenu] Start. Panel assigned? " + (settingsPanel != null));
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        Debug.Log("[SettingsMenu] OpenSettings CALLED");
        if (settingsPanel == null)
        {
            Debug.LogError("[SettingsMenu] settingsPanel is NULL! Assign it in the Inspector.");
            return;
        }
        settingsPanel.SetActive(true);
        Debug.Log("[SettingsMenu] Panel active state now: " + settingsPanel.activeSelf);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}