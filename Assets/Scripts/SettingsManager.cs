using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Audio")]
    public float musicVolume = 1f;
    public bool musicMuted = false;

    public float sfxVolume = 1f;
    public bool sfxMuted = false;

    [Header("References")]
    public AudioMixer audioMixer;
    public Image brightnessOverlay;

    [Header("Brightness")]
    public float brightness = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSettings();
            ApplySettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -------------------------
    // MUSIC (public setters)
    // -------------------------

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        ApplyMusicVolume();
        SaveSettings();
    }

    public void SetMusicMuted(bool value)
    {
        musicMuted = value;
        ApplyMusicVolume(); // mute is just volume = 0
        SaveSettings();
    }

    // -------------------------
    // SFX (public setters)
    // -------------------------

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        ApplySFXVolume();
        SaveSettings();
    }

    public void SetSFXMuted(bool value)
    {
        sfxMuted = value;
        ApplySFXVolume();
        SaveSettings();
    }

    // -------------------------
    // BRIGHTNESS
    // -------------------------

    public void SetBrightness(float value)
    {
        Debug.Log($"[SettingsManager] SetBrightness called with value: {value}");
        brightness = Mathf.Clamp(value, 0.2f, 1f);
        Debug.Log($"[SettingsManager] Brightness set to: {brightness}");
        PlayerPrefs.SetFloat("brightness", brightness);
        PlayerPrefs.Save();
        ApplyBrightness();          // ← THIS was missing
    }

    // -------------------------
    // APPLY (no saving)
    // -------------------------

    private void ApplySettings()
    {
        ApplyMusicVolume();
        ApplySFXVolume();
        ApplyBrightness();
    }

    private void ApplyMusicVolume()
    {
        if (audioMixer == null) return;
        float v = musicMuted ? 0f : musicVolume;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(v, 0.0001f)) * 20f);
    }

    private void ApplySFXVolume()
    {
        if (audioMixer == null) return;
        float v = sfxMuted ? 0f : sfxVolume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(v, 0.0001f)) * 20f);
    }

    private void ApplyBrightness()
    {
        //var overlay = GameObject.Find("BrightnessOverlay")?.GetComponent<Image>();
        //if (overlay != null)
        //{
        //    float alpha = 1f - brightness;                  // brightness 1 → alpha 0
        //    var c = overlay.color;
        //    overlay.color = new Color(c.r, c.g, c.b, alpha);
        //}
        float alpha = 1f - brightness;                  // brightness 1 → alpha 0
        Debug.Log($"[SettingsManager] Applying brightness with alpha: {alpha}");
        if(brightnessOverlay != null)
        {
            var c = brightnessOverlay.color;
            brightnessOverlay.color = new Color(c.r, c.g, c.b, alpha);
        }
        else
        {
            Debug.LogWarning("[SettingsManager] Brightness overlay is not assigned!");
        }
    }

    // -------------------------
    // SAVE
    // -------------------------

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("MusicMuted", musicMuted ? 1 : 0);

        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("SFXMuted", sfxMuted ? 1 : 0);

        PlayerPrefs.SetFloat("Brightness", brightness);

        PlayerPrefs.Save();
    }

    // -------------------------
    // LOAD
    // -------------------------

    private void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;

        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;

        brightness = PlayerPrefs.GetFloat("Brightness", 1f);
    }
}