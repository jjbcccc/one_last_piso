using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    public void OpenStageSelect()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void BackToMenuScreen()
    {
        SceneManager.LoadScene("MenuScreen");
    }
}
