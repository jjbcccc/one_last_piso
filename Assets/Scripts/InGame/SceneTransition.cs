using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find the fade image in the new scene
        if (fadeImage == null)
        {
            GameObject fade = GameObject.Find("FadeImage");
            if (fade != null)
                fadeImage = fade.GetComponent<Image>();
        }

        // Ensure fully transparent on scene start
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    public void RestartCurrentScene()
    {
        StartCoroutine(FadeAndReload(SceneManager.GetActiveScene().name));
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndReload(sceneName));
    }

    private IEnumerator FadeAndReload(string sceneName)
    {
        Time.timeScale = 0f;
        yield return StartCoroutine(FadeTo(1f));
        Time.timeScale = 1f;

        Debug.Log($"[SceneTransition] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("[SceneTransition] fadeImage is null — skipping fade.");
            yield break;
        }

        float startAlpha = fadeImage.color.a;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }

        Color final = fadeImage.color;
        final.a = targetAlpha;
        fadeImage.color = final;
    }

    public void FadeOutForDeath()
    {
        if (fadeImage != null) StartCoroutine(FadeTo(1f));
    }
}