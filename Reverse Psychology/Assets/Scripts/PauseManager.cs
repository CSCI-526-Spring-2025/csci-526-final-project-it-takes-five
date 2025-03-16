using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;

    private bool isPaused = false;

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    public void TogglePause()
    {
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
}
