using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial2Hint : MonoBehaviour
{
    // Reference to the PickupText GameObject (a TextMeshPro UI element).
    // Assign this in the Inspector.
    public GameObject PickupText;
    public GameObject JumpText;

    void Start()
    {
        // Hide the pickup text initially.
        if (PickupText != null)
        {
            PickupText.SetActive(false);
        }
        else
        {
            Debug.LogError("PickupText is not assigned in Tutorial2Hint!");
        }
    }

    void Update()
    {
        // When the F key is pressed, hide the PickupText if it's active.
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (PickupText != null && PickupText.activeSelf)
            {
                PickupText.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            JumpText.SetActive(false);
        }
    }

    // Called when another collider enters this GameObject's trigger collider.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has the tag "BlueOrb".
        if (collision.CompareTag("BlueOrb"))
        {
            if (PickupText != null)
            {
                PickupText.SetActive(true);
            }
        }
    }

    // Called when another collider exits this GameObject's trigger collider.
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Hide the text when the blue orb is no longer colliding.
        if (collision.CompareTag("BlueOrb"))
        {
            if (PickupText != null)
            {
                PickupText.SetActive(false);
            }
        }
    }
}
