using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene loading
using System.Collections;

public class DoorLevelScript : MonoBehaviour
{
    public Color openColor = Color.green;
    private bool isOpen = false;
    private SpriteRenderer sr;
    private BoxCollider2D col;
    public GameObject gameOverText; 


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
            sr.color = openColor; // Change door color to green when opened

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
            Debug.Log("Tutorial 1 Complete");

            StartCoroutine(ShowGameOverAndLoadScene());
            // Load the Tutorials Scene (make sure the scene name is correct in Unity's Build Settings)
            //SceneManager.LoadScene("MainMenuScene");
        }
    }
    IEnumerator ShowGameOverAndLoadScene()
    {
        // Display the "Game Over" message.
        // Option 1: Enable a UI panel that displays the message:
        // if (gameOverPanel != null)
        //     gameOverPanel.SetActive(true);

        if (gameOverText != null)
            gameOverText.SetActive(true);

        // Option 2: Use a debug log or any other visual cue if no UI is available.
        Debug.Log("Game Over");

        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);

        if (gameOverText != null)
            gameOverText.SetActive(false);

        MenuController.showTutorialsPanel = false;
        MenuController.showLevelsPanel = true;

        // Load the Main Menu Scene after the delay
        SceneManager.LoadScene("MainMenuScene");
    }
}
