using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Assign these in the Inspector
    public GameObject mainMenuPanel;    // The panel with "Tutorials" and "Levels" buttons
    public GameObject tutorialsPanel;   // The panel with the three tutorial buttons
    public GameObject levelsPanel;   // The panel with the levels buttons

    public static bool isTutorial = false;
    public static int currentLevel = 0;

    public static bool showTutorialsPanel = false;
    public static bool showLevelsPanel = false;


    void Start()
    {
        // Ensure only the main menu is active at game start
        mainMenuPanel.SetActive(true);
        tutorialsPanel.SetActive(false);
        levelsPanel.SetActive(false);


        if (showTutorialsPanel)
        {
            OnTutorialsClicked();
            // Reset the flag after using it
            showTutorialsPanel = false;
        }
        if (showLevelsPanel)
        {
            OnLevelsClicked();
            // Reset the flag after using it
            showLevelsPanel = false;
        }
    }

    // Called when the "Tutorials" button is clicked
    public void OnTutorialsClicked()
    {
        mainMenuPanel.SetActive(false);  // Hide the main menu
        tutorialsPanel.SetActive(true);    // Show the tutorials menu
        levelsPanel.SetActive(false);

    }

    // Called when the "Levels" button is clicked (for later use)
    public void OnLevelsClicked()
    {
        mainMenuPanel.SetActive(false);  // Hide the main menu
        tutorialsPanel.SetActive(false);    // Show the tutorials menu
        levelsPanel.SetActive(true);

    }

    public void OnLevel1Clicked()
    {
        isTutorial = false;
        currentLevel = 1;
        Debug.Log("Level 1 selected. Load Level content here.");
        SceneManager.LoadScene("Level1");
    }

    public void OnLevel2Clicked()
    {
        isTutorial = false;
        currentLevel = 2;
        Debug.Log("Level 2 selected. Load Level content here.");
        SceneManager.LoadScene("Level2");
    }

    // Called when "Tutorial 1" button is clicked
    public void OnTutorial1Clicked()
    {
        isTutorial=true;
        currentLevel = 1;
        // For now, just log the action; later you can load a new scene or display your tutorial
        Debug.Log("Tutorial 1 selected. Load tutorial content here.");
        SceneManager.LoadScene("Tutorial1Scene");
    }

    // Add similar methods for Tutorial 2 and 3 if needed
    public void OnTutorial2Clicked()
    {
        isTutorial = true;
        currentLevel = 2;
        Debug.Log("Tutorial 2 selected. Functionality to be added.");
        SceneManager.LoadScene("Tutorial2Scene");
    }

    public void OnTutorial3Clicked()
    {
        isTutorial = true;
        currentLevel = 3;
        Debug.Log("Tutorial 3 selected. Functionality to be added.");
        SceneManager.LoadScene("Tutorial3Scene");
    }
}
