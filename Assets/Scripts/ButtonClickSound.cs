using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(AudioSource))]
public class ButtonClickSound : MonoBehaviour
{
    private AudioSource source;
    private Button button;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        button = GetComponent<Button>();

        Debug.Log($"[ButtonClickSound] Awake on {gameObject.name}. " +
                  $"clip={(source.clip != null ? source.clip.name : "NULL")}, " +
                  $"output={(source.outputAudioMixerGroup != null ? source.outputAudioMixerGroup.name : "NULL")}");

        button.onClick.AddListener(PlayClick);
    }

    void PlayClick()
    {
        Debug.Log($"[ButtonClickSound] Click detected on {gameObject.name}!");
        if (source != null && source.clip != null)
            source.PlayOneShot(source.clip);
    }
}   