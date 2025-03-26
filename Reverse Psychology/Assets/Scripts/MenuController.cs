using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Assign these in the Inspector
    public GameObject mainMenuPanel;    // The panel with and "Play" button
    public GameObject levelsPanel;   // The panel with the all levels buttons

    public static int currentLevel = 0;
    public static bool showLevelsPanel = false;

    void Start()
    {
        // Ensure only the main menu is active at game start
        mainMenuPanel.SetActive(true);
        levelsPanel.SetActive(false);

        if (showLevelsPanel)
        {
            OnLevelsClicked();
            // Reset the flag after using it
            showLevelsPanel = false;
        }
    }

    public void OnMainMenuBackClicked()
    {
        mainMenuPanel.SetActive(true);  // Hide the main menu
        levelsPanel.SetActive(false);
    }


    // Called when the "Levels" button is clicked (for later use)
    public void OnLevelsClicked()
    {
        mainMenuPanel.SetActive(false);  // Hide the main menu
        levelsPanel.SetActive(true);
        currentLevel = 0;

    }

    public void OnLevel1Clicked()
    {
        currentLevel = 1;
        Debug.Log("Level 1 selected. Load Level content here.");
        SceneManager.LoadScene("Level1");
    }

    public void OnLevel2Clicked()
    {
        currentLevel = 2;
        Debug.Log("Level 2 selected. Load Level content here.");
        SceneManager.LoadScene("Level2");
    }

    public void OnLevel3Clicked()
    {
        currentLevel = 3;
        Debug.Log("Level 3 selected. Load Level content here.");
        SceneManager.LoadScene("Level3");
    }

    public void OnLevel4Clicked()
    {
        currentLevel = 4;
        Debug.Log("Level 4 selected. Load Level content here.");
        SceneManager.LoadScene("Level4");
    }

    public void OnLevel5Clicked()
    {
        currentLevel = 5;
        Debug.Log("Level 5 selected. Load Level content here.");
        SceneManager.LoadScene("Level5");
    }

    public void OnLevel6Clicked()
    {
        currentLevel = 6;
        Debug.Log("Level 6 selected. Load Level content here.");
        SceneManager.LoadScene("Level6");
    }
}
