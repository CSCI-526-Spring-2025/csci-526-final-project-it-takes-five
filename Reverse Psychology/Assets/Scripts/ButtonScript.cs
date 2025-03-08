using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public DoorScript door;
    // Assign the Door GameObject (with DoorScript) in the Inspector.
    public GameObject instructionText;
    public GameObject Panel2;
    public PlayerController player;       // Assign the Player GameObject (with PlayerController) in the Inspector.
    public Color pressedColor = Color.gray;  // Color to change to when the button is pressed.

    private bool activated = false;
    private SpriteRenderer sr;

    public GameAnalytics gameAnalytics;

    void Start()
    {
        // Get the SpriteRenderer component from the button.
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated && collision.CompareTag("Player"))
        {
            activated = true;

            gameAnalytics.StartReturnJourney();

            // Change the button color to indicate it has been pressed.
            if (sr != null)
            {
                sr.color = pressedColor;
            }

            // Show text if it exists
            if (instructionText != null) instructionText.SetActive(true);
            if (Panel2 != null) Panel2.SetActive(true);

            
            // Trigger the door opening and player transformation.
            door.OpenDoor();
            player.TransformToHuman();
            Debug.Log("Button pressed: Door opened, player transformed, and button color changed.");
        }
    }
}
