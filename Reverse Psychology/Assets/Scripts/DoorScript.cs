using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene loading

public class DoorScript : MonoBehaviour
{
    public Color closedColor = Color.red;
    public Color openColor = Color.green;
    private bool isOpen = false;
    private SpriteRenderer sr;
    private BoxCollider2D col;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        sr.color = closedColor;
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

            // Load the Tutorials Scene (make sure the scene name is correct in Unity's Build Settings)
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}
