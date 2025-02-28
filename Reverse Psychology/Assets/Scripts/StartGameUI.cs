using UnityEngine;

public class StartGameUI : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown) // Detect any key press
        {
            gameObject.SetActive(false); // Hide the panel
        }
    }
}
