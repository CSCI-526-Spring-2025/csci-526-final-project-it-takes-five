using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OrbMovement : MonoBehaviour
{
    public float moveSpeed = 5f;    // Speed of the orb's movement
    private Vector2 target;       // The target stack position
    private bool isMoving = false;  // Tracks if the orb should move
    public GameObject Player;
    public bool doActive;
  
    public void MoveToStack(Vector2 pos, bool active)
    {
        target = pos;
        isMoving = true;
        doActive = active;
    }

    void Update()
    {
        if (isMoving && target != null)
        {
            // Smooth movement using Lerp
            transform.position = Vector2.Lerp(transform.position, target, moveSpeed * Time.deltaTime);

            // Check if the orb has reached the target position
            if (Vector2.Distance(transform.position, target) < 0.1f)
            {
                isMoving = false; // Stop moving
                if (doActive)
                {
                    gameObject.SetActive(false); // Hide the orb after reaching the stack
                }
                Player.GetComponent<PlayerController>().UpdateOrbUI();
            }
        }
    }
}
