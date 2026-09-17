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
        var sm = SettingsManager.Instance;
        if (sm == null)
        {
            Debug.LogError("SettingsManager.Instance is null!");
            return;
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = sm.musicVolume;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        if (musicMuteToggle != null)
        {
            musicMuteToggle.isOn = sm.musicMuted;
            musicMuteToggle.onValueChanged.AddListener(OnMusicMuteChanged);
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sm.sfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        if (sfxMuteToggle != null)
        {
            sfxMuteToggle.isOn = sm.sfxMuted;
            sfxMuteToggle.onValueChanged.AddListener(OnSFXMuteChanged);
        }
        if (brightnessSlider != null)
        {
            brightnessSlider.value = sm.brightness;
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        }
    }

    void OnMusicVolumeChanged(float value) => SettingsManager.Instance.SetMusicVolume(value);
    void OnMusicMuteChanged(bool value) => SettingsManager.Instance.SetMusicMuted(value);
    void OnSFXVolumeChanged(float value) => SettingsManager.Instance.SetSFXVolume(value);
    void OnSFXMuteChanged(bool value) => SettingsManager.Instance.SetSFXMuted(value);
    //void OnBrightnessChanged(float value) => SettingsManager.Instance.SetBrightness(value);
    void OnBrightnessChanged(float value)
    {
        SettingsManager.Instance.SetBrightness(value);
    }
}