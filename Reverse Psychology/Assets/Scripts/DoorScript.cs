using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene loading
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public Color openColor = Color.green;
    private bool isOpen = false;
    private SpriteRenderer sr;
    private BoxCollider2D col;
    public GameObject youWinText;
    private int maxLevels = 5;
    public GameAnalytics gameAnalytics;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            // sr.color = openColor; // Change door color to green when opened

            // Set collider as trigger so the player can pass through
            if (col != null)
            {
                col.isTrigger = true;
            }

            Debug.Log("Door opened!");
        }
    }

    // Detect when the player passes through the door
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen && collision.CompareTag("Player"))
        {
            Debug.Log("Level 1 Complete");
            gameAnalytics.EndLevel();
            StartCoroutine(ShowGameOverAndLoadScene());
            // Load the Tutorials Scene (make sure the scene name is correct in Unity's Build Settings)
            //SceneManager.LoadScene("MainMenuScene");
        }
    }
    IEnumerator ShowGameOverAndLoadScene()
    {

        if (youWinText != null)
            youWinText.SetActive(true);

        // Option 2: Use a debug log or any other visual cue if no UI is available.
        Debug.Log("You Win!");
        Time.timeScale = 0f;

        // Wait for 2 seconds in real time, even though the game is paused.
        yield return new WaitForSecondsRealtime(2f);

        if (youWinText != null)
            youWinText.SetActive(false);

        MenuController.showLevelsPanel = true;
        Time.timeScale = 1f;
        if (MenuController.currentLevel == maxLevels)
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            MenuController.currentLevel += 1;
            SceneManager.LoadScene("Level" + MenuController.currentLevel);
        }
    }
}
