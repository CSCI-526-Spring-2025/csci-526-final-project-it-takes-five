using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial2Hint : MonoBehaviour
{
    public GameObject PickupText; // Text shown when colliding with BlueOrb
    public GameObject JumpText;   // Text shown when colliding with Button

    private bool isNearOrb = false;  // Track if player is near BlueOrb
    private bool isNearButton = false; // Track if player is near Button

    void Start()
    {
        // Hide texts initially
        if (PickupText != null) PickupText.SetActive(false);
        if (JumpText != null) JumpText.SetActive(false);
    }

    void Update()
    {
        // Hide JumpText when space is pressed and player is near button
        if (isNearButton && Input.GetKeyDown(KeyCode.Space))
        {
            if (JumpText != null)
                JumpText.SetActive(false);

            isNearButton = false; // Reset the button collision flag
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Show PickupText when colliding with BlueOrb
        if (collision.CompareTag("BlueOrb"))
        {
            if (PickupText != null)
                PickupText.SetActive(true);

            isNearOrb = true;
        }

        // Show JumpText when colliding with Button
        if (collision.CompareTag("Button"))
        {
            if (JumpText != null)
                JumpText.SetActive(true);

            isNearButton = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Hide PickupText when player moves away from BlueOrb
        if (collision.CompareTag("BlueOrb"))
        {
            if (PickupText != null)
                PickupText.SetActive(false);

            isNearOrb = false;
        }
    }
}
