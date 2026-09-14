using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HowToPlayUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject howToPlayPanel;

    [Header("Page Content")]
    public TMP_Text descriptionText;

    [Header("Buttons")]
    public Button backButton;
    public Button nextButton;

    private int currentPage = 0;

    private string[] pages =
    {
        "Welcome to One Last Piso!\n\n" +
        "Your goal is to complete each level " +
        "and overcome the challenges presented to you.",

        "Explore the level and pay attention to your surroundings.\n\n" +
        "Use the available mechanics and interact with objects " +
        "to progress through the level.",

        "Reach the goal to complete the level.\n\n" +
        "Good luck and enjoy One Last Piso!"
    };

    private void Start()
    {
        ShowPage(0);
        howToPlayPanel.SetActive(false);
    }

    public void OpenHowToPlay()
    {
        currentPage = 0;

        howToPlayPanel.SetActive(true);

        ShowPage(currentPage);
    }

    public void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    private void ShowPage(int page)
    {
        descriptionText.text = pages[page];

        backButton.interactable = page > 0;
        nextButton.interactable = page < pages.Length - 1;
    }
}