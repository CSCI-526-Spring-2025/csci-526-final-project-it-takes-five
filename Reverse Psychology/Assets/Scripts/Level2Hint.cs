using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Hint : MonoBehaviour
{
    // Reference to the DropText GameObject (a TextMeshPro UI element).
    // Assign this in the Inspector.
    public GameObject DropText;
    private int flag = 0;

    void Start()
    {
        // Hide the drop text initially if flag is 0.
        if (flag == 0)
        {
            DropText.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && flag == 0)
        {
            flag = 1;
            // get orb stack from player controller script
            // Display the drop text if there are not two blue orbs at top of stack
            DropText.SetActive(true);
        }

        if (Input.GetMouseButtonDown(1))
        {
            DropText.SetActive(false);
        }
    }
}
