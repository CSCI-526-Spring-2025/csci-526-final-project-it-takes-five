using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Hint : MonoBehaviour
{
    // Reference to the DashText GameObject (a TextMeshPro UI element).
    // Assign this in the Inspector.
    public GameObject DashText;

    void Start()
    {
        // Hide the dash text initially.
        if (DashText != null)
        {
            DashText.SetActive(false);
        }
        else
        {
            Debug.LogError("DashText is not assigned in Level3Hint!");
        }
    }

    void Update()
    {
        // When Left Shift is pressed, hide the DashText if it's active.
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (DashText != null && DashText.activeSelf)
            {
                DashText.SetActive(false);
            }
        }
    }

    // Called when another collider enters this GameObject's trigger collider.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is the Button.
        if (collision.CompareTag("Button"))
        {
            if (DashText != null)
            {
                DashText.SetActive(true);
            }
        }
    }
}
