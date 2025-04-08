using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private float jumpForce = 7f;
    public Color ghostColor = Color.white;
    public Color humanColor = Color.yellow;

    private bool isGhost = true;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // Orb stack: capacity is 5.
    public Stack<GameObject> orbStack = new Stack<GameObject>();
    private int orbStackCapacity = 5;

    // UI element to display orb count and warning messages
    public TextMeshProUGUI orbStackUIText;
    private TextMeshProUGUI warningText;

    // Optional ground check settings.
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private Collider2D col;
    private List<Collider2D> wallColliders = new List<Collider2D>();
    private GameObject nearbyOrb;

    // UI elements to display the orb stack.
    public GameObject[] orbSlots;  
    public Sprite emptyTopSlotSprite; 
    public Sprite emptyBottomSlotSprite;

    // Dash variables.
    private bool isDashing = false;
    public float dashDuration = 0.2f;
    public float dashDistance = 7f;
    private float lastHorizontalInput = 1f; // Defaults to right.

    // Ability success variables
    public GameObject gameAnalytics;
    private bool jumpInProgress = false;
    private bool dashInProgress = false;
    private bool healInProgress = false;
    private string currentAbility = "";

    // At the class level, maintain a list of dropped orbs.
    public List<GameObject> droppedOrbs = new List<GameObject>();

    // Define a threshold for determining overlap.
    public float overlapThreshold = 0.1f;  
    private float shiftOffset = 0.6f;
    public Transform leftWall;           // Left boundary
    public Transform rightWall;
 
    bool IsOrbAtPosition(Vector3 pos)
    {
        foreach (GameObject orb in droppedOrbs)
        {
            // Check if the orb is close enough to be considered overlapping.
            if (Vector3.Distance(orb.transform.position, pos) < overlapThreshold)
            {
                return true;
            }
        }
        return false;
    }

    // Optionally, shift an orb if it is overlapping the drop position.
    void ResolveOverlaps(Vector3 dropPosition, GameObject topOrb)
    {
        bool check = true;
        foreach (GameObject orb in droppedOrbs.ToList())
        {
            // Check if the orb is overlapping.
            if (Vector3.Distance(orb.transform.position, dropPosition) < overlapThreshold)
            {
                check = false;

                // Calculate the potential new position.
                Vector3 newPosition = orb.transform.position + new Vector3(shiftOffset, 0, 0);

                // // Check boundary conditions to prevent going out of bounds.
                // float rightBoundary = rightWall.position.x;  // Adjust based on your game level dimensions.
                // float leftBoundary = leftWall.position.x;
                // if (newPosition.x > rightBoundary || newPosition.x < leftBoundary)
                // {
                //     // If shifting right is not feasible, try shifting left.
                //     newPosition = orb.transform.position - new Vector3(shiftOffset, 0, 0);
                // }

                // // Obstacle collision check.
                // Collider2D[] colliders = Physics2D.OverlapCircleAll(newPosition, overlapThreshold);
                // bool isObstacle = colliders.Any(col => col.CompareTag("Wall") || col.CompareTag("Obstacle"));
                // if (isObstacle)
                // {
                //     // Try shifting in the opposite direction if blocked by an obstacle.
                //     newPosition = orb.transform.position - new Vector3(shiftOffset, 0, 0);

                //     // Check the new position for obstacles.
                //     colliders = Physics2D.OverlapCircleAll(newPosition, overlapThreshold);
                //     isObstacle = colliders.Any(col => col.CompareTag("Wall") || col.CompareTag("Obstacle"));

                //     if (isObstacle)
                //     {
                //         // If both right and left are blocked, move the orb vertically to clear the way.
                //         newPosition = orb.transform.position + new Vector3(0, shiftOffset, 0);
                //     }
                // }

                // Update the orb's position.
                orb.transform.position = newPosition;

                // Recurse to resolve any overlaps caused by this movement.
                droppedOrbs.Add(topOrb);
                droppedOrbs.Remove(orb);
                ResolveOverlaps(newPosition, orb);
            }
        }
        if (check)
        {
            droppedOrbs.Add(topOrb);
        }
    }



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Start as ghost.
        gameObject.layer = LayerMask.NameToLayer("Ghost");
        rb.gravityScale = 0;
        sr.color = ghostColor;

        droppedOrbs = GameObject.FindGameObjectsWithTag("BlueOrb")
                                        .Union(GameObject.FindGameObjectsWithTag("YellowOrb")).ToList();

        // Disable collisions with walls.
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            Collider2D wallCol = wall.GetComponent<Collider2D>();
            if (wallCol != null)
            {
                wallColliders.Add(wallCol);
                Physics2D.IgnoreCollision(col, wallCol, true);
            }
        }
        UpdateOrbUI();
    }

    bool IsOverlapping(GameObject obj, Transform player)
    {
        Bounds objectBounds = obj.GetComponent<Renderer>().bounds;
        Bounds playerBounds = player.GetComponent<Renderer>().bounds;
        return objectBounds.Intersects(playerBounds);
    }

    bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }


    public Stack<GameObject> getStack()
    {
        return orbStack;
    }

    public bool abilityEnd() {
        // if(abilityInProgress == false) return false;
        // abilityInProgress = false;
        Debug.Log("currentAbility: " + currentAbility + ", jumpInProgress: " + jumpInProgress + ", dashInProgress: " + dashInProgress + ", healInProgress: " + healInProgress);
        if (jumpInProgress == false && dashInProgress == false && healInProgress == false) return false;
        if (jumpInProgress && currentAbility == "Jump") jumpInProgress = false;
        if (dashInProgress && currentAbility == "Dash") dashInProgress = false;
        if (healInProgress && currentAbility == "Heal") healInProgress = false;
       
       // Check if touching safe zone
        GameObject[] safeZones = GameObject.FindGameObjectsWithTag("SafeZone");
        GameAnalytics analyticsScript = gameAnalytics.GetComponent<GameAnalytics>();
        Debug.Log("Ability ended at: " + transform.position.x + ", " + transform.position.y);
        foreach (GameObject safeZone in safeZones)
        {
            if (IsOverlapping(safeZone, transform))
            {   
                analyticsScript.EndAbility(currentAbility, true, transform.position.x, transform.position.y);                
                return true;
            }
        }
        analyticsScript.EndAbility(currentAbility, false, transform.position.x, transform.position.y);
        return false;
    }

    void Update()
    {
        
        // Pick up orbs when pressing left arrow key.
        if (Input.GetMouseButtonDown(0))
        {
            if (nearbyOrb != null && orbStack.Count < orbStackCapacity)
            {
                droppedOrbs.Remove(nearbyOrb);
                orbStack.Push(nearbyOrb);
                // Deactivate the orb.
                OrbMovement movement = nearbyOrb.GetComponent<OrbMovement>();
                if (movement != null) { 
                    movement.MoveToStack(new Vector2(8.75f, 6.5f), true);
                } else
                {
                    nearbyOrb.SetActive(false);
                    UpdateOrbUI();
                }
                // Find other nearby orbs if they exist, otherwise set nearbyOrb to null.
                GameObject[] orbs = GameObject.FindGameObjectsWithTag("BlueOrb")
                                        .Union(GameObject.FindGameObjectsWithTag("YellowOrb")).ToArray();
                nearbyOrb = orbs.FirstOrDefault(orb => orb.activeInHierarchy && IsOverlapping(orb, transform));
            }
        }

        // Jump in human mode (when not a ghost) using Space.
        if (!isGhost && Input.GetKeyDown(KeyCode.Space))
        {
            if (orbStack.Count >= 2)
            {
                GameObject topOrb = orbStack.Pop();
                GameObject secondOrb = orbStack.Pop();

                if (topOrb.CompareTag("BlueOrb") && secondOrb.CompareTag("BlueOrb"))
                {
                    UpdateOrbUI();
                    // End old ability
                    if (jumpInProgress || dashInProgress || healInProgress) {
                        Debug.Log("Ability ended by starting new ability. Success: " + abilityEnd());
                    }
                    jumpInProgress = true;
                    currentAbility = "Jump";
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    Debug.Log("Jump performed using 2 blue orbs.");
                }
                else
                {
                    orbStack.Push(secondOrb);
                    orbStack.Push(topOrb);
                    Debug.Log("Jump cannot be performed without 2 blue orbs on top.");
                    if (warningText != null)
                    {
                        warningText.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                Debug.Log("Not enough orbs to jump.");
                if (warningText != null)
                {
                    warningText.gameObject.SetActive(true);
                }
            }
        }

        // Dash: when Left Shift is pressed and we're not already dashing.
        if (!isGhost && Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (orbStack.Count >= 2)
            {
                // Remove top two orbs.
                GameObject topOrb = orbStack.Pop();
                GameObject secondOrb = orbStack.Pop();

                if (topOrb.CompareTag("YellowOrb") && secondOrb.CompareTag("YellowOrb"))
                {
                    UpdateOrbUI();
                    if (jumpInProgress || dashInProgress || healInProgress) {
                        Debug.Log("Ability ended by starting new ability. Success: " + abilityEnd());
                    }
                    dashInProgress = true;
                    currentAbility = "Dash";
                    Debug.Log("Dash performed using 2 yellow orbs.");
                    StartCoroutine(Dash());
                }
                else
                {
                    // The two top orbs are not both yellow. Push them back.
                    orbStack.Push(secondOrb);
                    orbStack.Push(topOrb);
                    Debug.Log("Dash cannot be performed without 2 yellow orbs on top.");
                    if (warningText != null)
                    {
                        warningText.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                Debug.Log("Not enough orbs to dash.");
                if (warningText != null)
                {
                    warningText.gameObject.SetActive(true);
                }
            }
        }

        // Only process regular movement if not dashing.
        if (!isDashing)
        {
            float moveInput = Input.GetAxis("Horizontal");
            if (moveInput != 0)
            {
                lastHorizontalInput = moveInput;
            }
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }

        // Press G to drop the top orb at the player's position.
        // if (Input.GetMouseButtonDown(1))
        // {
        //     if (orbStack.Count > 0)
        //     {
        //         GameObject topOrb = orbStack.Pop();
        //         UpdateOrbUI();
        //         topOrb.SetActive(true);
        //         Vector3 dropPosition = transform.position;
        //         dropPosition.y -= 0.3f; // Adjust the drop position downward by 0.2 units.
        //         OrbMovement movement = topOrb.GetComponent<OrbMovement>();
        //         if (movement != null)
        //         {
        //             movement.MoveToStack(dropPosition, false);
        //             topOrb.SetActive(true);
        //         }
        //         else
        //         {
        //             topOrb.transform.position = dropPosition;
        //         }       
        //     }
        // }


            if (Input.GetMouseButtonDown(1))
            {
                if (orbStack.Count > 0)
                {
                    GameObject topOrb = orbStack.Pop();
                    UpdateOrbUI();
                    topOrb.SetActive(true);

                    // Determine the base drop position relative to the player.
                    Vector3 dropPosition = transform.position;
                    dropPosition.y -= 0.3f; // Adjust the drop position downward.

                   

                    // Now place the new orb at the drop position.
                    OrbMovement movement = topOrb.GetComponent<OrbMovement>();
                    if (movement != null)
                    {
                        movement.MoveToStack(dropPosition, false);
                    }
                    else
                    {
                        topOrb.transform.position = dropPosition;
                    }

                    // Resolve overlap for any already dropped orbs at the intended drop position.
                    ResolveOverlaps(dropPosition, topOrb);

                    // Add the new orb to the list so future drops can check it.
                    
                }
            }


        // If jump is in progress, check if it has ended.
        if (jumpInProgress && Mathf.Abs(rb.velocity.y) < 0.01f && IsGrounded()) {
            Debug.Log("Ground detected? " + IsGrounded());
            Debug.Log("Jump ended by touching ground. Success: " + abilityEnd());
            jumpInProgress = false;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        // Calculate dash speed so that the player covers dashDistance in dashDuration.
        float dashSpeed = dashDistance / dashDuration;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0; // Disable gravity for a pure horizontal dash.
        rb.velocity = new Vector2(lastHorizontalInput * dashSpeed, 0);
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;
        Debug.Log("Ability ended by completing dash. Success: " + abilityEnd());
    }

    // Transform the player from ghost to human.
    public void TransformToHuman()
    {
        if (isGhost)
        {
            isGhost = false;
            rb.gravityScale = 1;
            foreach (Collider2D wallCol in wallColliders)
            {
                Physics2D.IgnoreCollision(col, wallCol, false);
            }
            gameObject.layer = LayerMask.NameToLayer("Human");
            sr.color = humanColor;
            Debug.Log("Player transformed to human!");
        }
    }

    // When the player enters an orb's trigger zone, store a reference.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BlueOrb") || collision.CompareTag("YellowOrb"))
        {
            if (nearbyOrb == null)
            {
                nearbyOrb = collision.gameObject;
                //Debug.Log("Orb in pickup range. Press F to pick it up.");
            }
        }
    }

    // Clear the orb reference when the player leaves the orb's trigger zone.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BlueOrb") || collision.CompareTag("YellowOrb"))
        {
            if (collision.gameObject == nearbyOrb)
            {
                nearbyOrb = null;
                Debug.Log("Orb left pickup range.");
            }
        }
    }

    // Update the UI for the orb stack.
    public void UpdateOrbUI()
    {   
        // Loop through each UI slot.
        for (int i = 0; i < orbSlots.Length; i++)
        {
            if (orbSlots[i] == null)
            {
                Debug.LogError("Orb slot at index " + i + " is null.");
                continue;
            }
            // Each orbSlot is a GameObject with an Image component.
            Image slotImage = orbSlots[i].GetComponent<Image>();

            if (i < orbStack.Count)
            {
                // Retrieve the orb from the stack using LINQ's ElementAt.
                GameObject orbInStack = orbStack.ElementAt(i);
                OrbClass orbComponent = orbInStack.GetComponent<OrbClass>();
                SpriteRenderer orbSpriteRenderer = orbInStack.GetComponent<SpriteRenderer>();
                if (orbSpriteRenderer != null)
                {
                    // Set the UI slot sprite and color to match the orb.
                    slotImage.sprite = orbSpriteRenderer.sprite;
                    slotImage.color = orbSpriteRenderer.color;
                }
            }
            else
            {
                // Set the slot to show the empty slot sprite and a default color (e.g., white).
                if(i < 2)
                {
                    slotImage.sprite = emptyTopSlotSprite;                    
                } else
                {
                    slotImage.sprite = emptyBottomSlotSprite;
                }
                slotImage.color = Color.white;
            }
        }
    }
}
