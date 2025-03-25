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

    public void TogglePause()
    {
        Debug.Log("TogglePause clicked");
        if (instructionsPanel.activeSelf)
            return;
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        TogglePause();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Ensure time resumes before restarting
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartFromCheckpoint()
    {
        Time.timeScale = 1f; // Ensure time resumes before respawning
        // Logic to reset to checkpoint
        GameObject.FindWithTag("Player").transform.position = new Vector3(2f, 1f, 0f); // Example checkpoint
        TogglePause();
    }


    public void ShowInstructions()
    {
        instructionsPanel.SetActive(true);
    }

    // Closes the instructions panel, leaving the pause menu active
    public void CloseInstructions()
    {
        instructionsPanel.SetActive(false);
    }
}
