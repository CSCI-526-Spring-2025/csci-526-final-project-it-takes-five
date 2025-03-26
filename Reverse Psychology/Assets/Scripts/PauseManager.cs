using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;

    private bool isPaused = false;
    public GameObject instructionsPanel;

    void Start()
    {
        pauseMenu.SetActive(false);
        instructionsPanel.SetActive(false);
    }

    public void PauseGame()
    {
        Debug.Log("Pause clicked");
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;

        // Disable button for a short duration to prevent double click
        StartCoroutine(PreventDoubleClick());
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;

        // Disable button for a short duration to prevent double click
        StartCoroutine(PreventDoubleClick());
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Ensure time resumes before restarting
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // public void RestartFromCheckpoint()
    // {
    //     Time.timeScale = 1f; // Ensure time resumes before respawning
    //     // Logic to reset to checkpoint
    //     GameObject.FindWithTag("Player").transform.position = new Vector3(2f, 1f, 0f); // Example checkpoint
    // }

    public void ShowInstructions()
    {
        instructionsPanel.SetActive(true);
    }

    // Closes the instructions panel, leaving the pause menu active
    public void CloseInstructions()
    {
        instructionsPanel.SetActive(false);
        // Ensure the game remains paused when instructions are closed
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;

        // Disable button for a short duration to prevent double click
        StartCoroutine(PreventDoubleClick());
    }

    private IEnumerator PreventDoubleClick()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null); // Deselect UI elements
    }
}
