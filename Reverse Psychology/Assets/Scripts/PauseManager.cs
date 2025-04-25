using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using ButtonScript.GameCheckpoint;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;

    private bool isPaused = false;
    public GameObject instructionsPanel;
    public GameObject button;
    public GameObject restartCheckpointButton;
    public GameObject checkpointLoadText;




    void Start()
    {
        pauseMenu.SetActive(false);
        instructionsPanel.SetActive(false);
        restartCheckpointButton.SetActive(false);
        if (checkpointLoadText != null)
            checkpointLoadText.SetActive(false);
    }

    void Update()
    {
        // When the player hits Esc, toggle pause/resume.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Debug.Log("Pause clicked");
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;

        if (button.GetComponent<ButtonScript>().checkpointCreated) // CHANGED
        {
            restartCheckpointButton.SetActive(true); // CHANGED
        }
        else
        {
            restartCheckpointButton.SetActive(false); // CHANGED
        }

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

    public void RestartFromCheckpoint()
    {
        //Debug.Log(button.GetComponent<ButtonScript>().getCheckpoint().playerPosition);
        //for(int i = 0; i < button.GetComponent<ButtonScript>().getCheckpoint().remainingOrbs.Count; i++) {
        //    Debug.Log()
        //}

        StartCoroutine(LoadCheckpointText());
        ResumeGame();
     
    }

    IEnumerator LoadCheckpointText()
    {
        if (checkpointLoadText != null)
            checkpointLoadText.SetActive(true);
        button.GetComponent<ButtonScript>().LoadCheckpoint();
        // Wait for 2 seconds in real time, even though the game is paused.
        yield return new WaitForSecondsRealtime(2f);
        if (checkpointLoadText != null)
            checkpointLoadText.SetActive(false);
    }

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
