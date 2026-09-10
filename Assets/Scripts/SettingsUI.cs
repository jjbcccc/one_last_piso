using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Music")]
    public Slider musicVolumeSlider;
    public Toggle musicMuteToggle;

    [Header("SFX")]
    public Slider sfxVolumeSlider;
    public Toggle sfxMuteToggle;

    [Header("Brightness")]
    public Slider brightnessSlider;

    void Start()
    {
        // Load current values into the UI
        var sm = SettingsManager.Instance;
        if (sm == null)
        {
            Debug.LogError("SettingsManager.Instance is null! Make sure it's in the scene.");
            return;
        }

        musicVolumeSlider.value = sm.musicVolume;
        musicMuteToggle.isOn = sm.musicMuted;
        sfxVolumeSlider.value = sm.sfxVolume;
        sfxMuteToggle.isOn = sm.sfxMuted;
        brightnessSlider.value = sm.brightness;

        // Hook up listeners
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        musicMuteToggle.onValueChanged.AddListener(OnMusicMuteChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        sfxMuteToggle.onValueChanged.AddListener(OnSFXMuteChanged);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
    }

    void OnMusicVolumeChanged(float value) => SettingsManager.Instance.SetMusicVolume(value);
    void OnMusicMuteChanged(bool value) => SettingsManager.Instance.SetMusicMuted(value);
    void OnSFXVolumeChanged(float value) => SettingsManager.Instance.SetSFXVolume(value);
    void OnSFXMuteChanged(bool value) => SettingsManager.Instance.SetSFXMuted(value);
    void OnBrightnessChanged(float value) => SettingsManager.Instance.SetBrightness(value);
}