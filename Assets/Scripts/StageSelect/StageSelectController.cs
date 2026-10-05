using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectController : MonoBehaviour
{
    [Header("Stage Buttons")]
    [SerializeField] private Button stage1Button;
    [SerializeField] private Button stage2Button;
    [SerializeField] private Button stage3Button;

    [Header("Stage 2 Lock UI")]
    [SerializeField] private GameObject stage2LockOverlay;
    [SerializeField] private GameObject stage2LockIcon;

    [Header("Stage 3 Lock UI")]
    [SerializeField] private GameObject stage3LockOverlay;
    [SerializeField] private GameObject stage3LockIcon;

    private void Start()
    {
        UpdateStageLocks();
    }

    private void UpdateStageLocks()
    {
        int highestUnlockedStage =
            StageProgress.GetHighestUnlockedStage();

        Debug.Log(
            "[StageSelect] Highest unlocked stage: " +
            highestUnlockedStage
        );

        // --------------------------------
        // STAGE 1
        // --------------------------------

        bool stage1Unlocked =
            StageProgress.IsStageUnlocked(1);

        stage1Button.interactable = stage1Unlocked;


        // --------------------------------
        // STAGE 2
        // --------------------------------

        bool stage2Unlocked =
            StageProgress.IsStageUnlocked(2);

        stage2Button.interactable = stage2Unlocked;

        if (stage2LockOverlay != null)
        {
            stage2LockOverlay.SetActive(!stage2Unlocked);
        }

        if (stage2LockIcon != null)
        {
            stage2LockIcon.SetActive(!stage2Unlocked);
        }


        // --------------------------------
        // STAGE 3
        // --------------------------------

        bool stage3Unlocked =
            StageProgress.IsStageUnlocked(3);

        stage3Button.interactable = stage3Unlocked;

        if (stage3LockOverlay != null)
        {
            stage3LockOverlay.SetActive(!stage3Unlocked);
        }

        if (stage3LockIcon != null)
        {
            stage3LockIcon.SetActive(!stage3Unlocked);
        }
    }

    // --------------------------------
    // STAGE BUTTONS
    // --------------------------------

    public void OpenStage1()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void OpenStage2()
    {
        if (!StageProgress.IsStageUnlocked(2))
        {
            Debug.Log(
                "[StageSelect] Stage 2 is locked."
            );

            return;
        }

        SceneManager.LoadScene("Stage2");
    }

    public void OpenStage3()
    {
        if (!StageProgress.IsStageUnlocked(3))
        {
            Debug.Log(
                "[StageSelect] Stage 3 is locked."
            );

            return;
        }

        SceneManager.LoadScene("Stage3");
    }
}