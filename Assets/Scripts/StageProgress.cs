using UnityEngine;

public static class StageProgress
{
    private const string HighestUnlockedStageKey = "HighestUnlockedStage";

    // Returns the highest stage the player has unlocked.
    // New players start with Stage 1 unlocked.
    public static int GetHighestUnlockedStage()
    {
        return PlayerPrefs.GetInt(HighestUnlockedStageKey, 1);
    }

    // Unlocks a stage if it is higher than the player's
    // current highest unlocked stage.
    public static void UnlockStage(int stageNumber)
    {
        int currentHighestStage = GetHighestUnlockedStage();

        if (stageNumber > currentHighestStage)
        {
            PlayerPrefs.SetInt(
                HighestUnlockedStageKey,
                stageNumber
            );

            PlayerPrefs.Save();

            Debug.Log(
                "[StageProgress] Stage " +
                stageNumber +
                " unlocked."
            );
        }
        else
        {
            Debug.Log(
                "[StageProgress] Stage " +
                stageNumber +
                " is already unlocked."
            );
        }
    }

    // Checks whether a particular stage is unlocked.
    public static bool IsStageUnlocked(int stageNumber)
    {
        return stageNumber <= GetHighestUnlockedStage();
    }

    // Used for testing/resetting progression during development.
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(HighestUnlockedStageKey);
        PlayerPrefs.Save();

        Debug.Log(
            "[StageProgress] Progress reset. " +
            "Stage 1 is now unlocked."
        );
    }
}