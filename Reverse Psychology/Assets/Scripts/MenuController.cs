using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Assign these in the Inspector
    public GameObject mainMenuPanel;    // The panel with "Tutorials" and "Levels" buttons
    public GameObject tutorialsPanel;   // The panel with the three tutorial buttons

    void Start()
    {
        // Ensure only the main menu is active at game start
        mainMenuPanel.SetActive(true);
        tutorialsPanel.SetActive(false);
    }

    // Called when the "Tutorials" button is clicked
    public void OnTutorialsClicked()
    {
        mainMenuPanel.SetActive(false);  // Hide the main menu
        tutorialsPanel.SetActive(true);    // Show the tutorials menu
    }

    // Called when the "Levels" button is clicked (for later use)
    public void OnLevelsClicked()
    {
        Debug.Log("Levels button clicked. Levels functionality to be added.");
    }

    // Called when "Tutorial 1" button is clicked
    public void OnTutorial1Clicked()
    {
        // For now, just log the action; later you can load a new scene or display your tutorial
        Debug.Log("Tutorial 1 selected. Load tutorial content here.");
        SceneManager.LoadScene("Tutorial1Scene");
    }

    // Add similar methods for Tutorial 2 and 3 if needed
    public void OnTutorial2Clicked()
    {
        Debug.Log("Tutorial 2 selected. Functionality to be added.");
        SceneManager.LoadScene("Tutorial2Scene");
    }

    public void OnTutorial3Clicked()
    {
        Debug.Log("Tutorial 3 selected. Functionality to be added.");
    }
}
