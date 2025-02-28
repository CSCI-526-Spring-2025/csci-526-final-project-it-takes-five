using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public DoorScript door;               // Assign the Door GameObject (with DoorScript) in the Inspector.
    public PlayerController player;       // Assign the Player GameObject (with PlayerController) in the Inspector.
    public Color pressedColor = Color.gray;  // Color to change to when the button is pressed.

    private bool activated = false;
    private SpriteRenderer sr;

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

            // Change the button color to indicate it has been pressed.
            if (sr != null)
            {
                sr.color = pressedColor;
            }

            // Trigger the door opening and player transformation.
            door.OpenDoor();
            player.TransformToHuman();
            Debug.Log("Button pressed: Door opened, player transformed, and button color changed.");
        }
    }
}
