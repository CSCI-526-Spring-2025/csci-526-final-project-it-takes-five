using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial3Hint : MonoBehaviour
{
    // Reference to the PickupText GameObject (a TextMeshPro UI element).
    // Assign this in the Inspector.
    public GameObject DropText;
    // public GameObject JumpText;

    void Start()
    {
        // Hide the drop text initially.
        if (DropText != null)
        {
            DropText.SetActive(false);
        }
        else
        {
            Debug.LogError("DropText is not assigned in Tutorial3Hint!");
        }
    }

    void Update()
    {
        // When the F key is pressed, hide the PickupText if it's active.
        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     if (PickupText != null && PickupText.activeSelf)
        //     {
        //         PickupText.SetActive(false);
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     JumpText.SetActive(false);
        // }
    }
}
