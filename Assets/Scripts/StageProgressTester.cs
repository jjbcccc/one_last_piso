using UnityEngine;
using UnityEngine.InputSystem;

public class StageProgressTester : MonoBehaviour
{
    private const int MaxStageInGame = 3;

    private void Start()
    {
        Debug.Log(
            "[StageProgressTester] Highest unlocked stage: " +
            StageProgress.GetHighestUnlockedStage()
        );
    }

    private void Update()
    {
        // Press U to unlock the next stage.
        if (Keyboard.current != null &&
            Keyboard.current.uKey.wasPressedThisFrame)
        {
            int current = StageProgress.GetHighestUnlockedStage();

            if (current >= MaxStageInGame)
            {
                Debug.Log(
                    "[StageProgressTester] All stages already unlocked (max = " +
                    MaxStageInGame + ")."
                );
            }
            else
            {
                int nextStage = current + 1;
                StageProgress.UnlockStage(nextStage);

                Debug.Log(
                    "[StageProgressTester] Unlocked stage " + nextStage +
                    ". Highest unlocked is now: " +
                    StageProgress.GetHighestUnlockedStage()
                );
            }
        }

        // Press R to reset progression.
        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            StageProgress.ResetProgress();

            Debug.Log(
                "[StageProgressTester] Highest unlocked stage after reset: " +
                StageProgress.GetHighestUnlockedStage()
            );
        }
    }
}