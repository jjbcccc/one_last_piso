using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    [Header("Stage Buttons")]
    public Button stage1Button;
    public Button stage2Button;
    public Button stage3Button;

    private void Start()
    {
        // Stage 1 is unlocked by default.
        stage1Button.interactable = true;

        // Stage 2 and Stage 3 are locked for now.
        stage2Button.interactable = false;
        stage3Button.interactable = false;
    }

    public void OpenStage1()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void OpenStage2()
    {
        SceneManager.LoadScene("Stage2");
    }

    public void OpenStage3()
    {
        SceneManager.LoadScene("Stage3");
    }
}