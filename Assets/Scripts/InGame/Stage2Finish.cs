using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage2Finish : MonoBehaviour
{
    [SerializeField] private GameObject finishPage;
    //[SerializeField] private string nextStageScene = "Stage2";

    private bool stageFinished = false;

    private void Start()
    {
        Time.timeScale = 1f;
        stageFinished = false;

        if (finishPage != null)
            finishPage.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (stageFinished) return;
        if (!other.CompareTag("Player")) return;

        stageFinished = true;
        StageProgress.UnlockStage(3); // Unlock Stage 3 when Stage 2 is finished
        Debug.Log("Unclocked Stage: " + StageProgress.GetHighestUnlockedStage());

        if (finishPage != null)
            finishPage.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;
        if (finishPage != null) finishPage.SetActive(false);

        try
        {
            int idx = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(idx);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FIX] LoadScene FAILED: {e.Message}");
        }
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageSelect");
    }

    public void NextStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Stage3");
    }
}