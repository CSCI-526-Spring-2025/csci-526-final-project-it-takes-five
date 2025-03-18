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
    public TextMeshProUGUI warningText;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Start as ghost.
        gameObject.layer = LayerMask.NameToLayer("Ghost");
        rb.gravityScale = 0;
        sr.color = ghostColor;

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

    bool IsOverlapping(GameObject orb, Transform player)
    {
        Bounds orbBounds = orb.GetComponent<Renderer>().bounds;
        Bounds playerBounds = player.GetComponent<Renderer>().bounds;
        return orbBounds.Intersects(playerBounds);
    }

    public Stack<GameObject> getStack()
    {
        return orbStack;
    }

    void Update()
    {
        
        // Pick up orbs when pressing F.
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Pressing F");
            if (nearbyOrb != null && orbStack.Count < orbStackCapacity)
            {
                //Debug.Log("Inside if");
                orbStack.Push(nearbyOrb);
                nearbyOrb.SetActive(false); // Deactivate the orb.
                Debug.Log("Picked up orb. Orb count: " + orbStack.Count);
                //Debug.Log("Orb id: " + nearbyOrb.uniqueID);
                UpdateOrbUI();
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
                Debug.Log("Not enough orbs to dash.");
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
        if (Input.GetMouseButtonDown(1))
        {
            if (orbStack.Count > 0)
            {
                GameObject topOrb = orbStack.Pop();
                Vector3 dropPosition = transform.position;
                dropPosition.y -= 0.3f; // Adjust the drop position downward by 0.2 units.
                topOrb.transform.position = dropPosition;
                topOrb.SetActive(true);
                UpdateOrbUI();
            }
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
    private void UpdateOrbUI()
    {
        // Loop through each UI slot.
        for (int i = 0; i < orbSlots.Length; i++)
        {
            // Each orbSlot is a GameObject with an Image component.
            Image slotImage = orbSlots[i].GetComponent<Image>();

            if (i < orbStack.Count)
            {
                // Retrieve the orb from the stack using LINQ's ElementAt.
                GameObject orbInStack = orbStack.ElementAt(i);
                OrbClass orbComponent = orbInStack.GetComponent<OrbClass>();
                Debug.Log(orbComponent.uniqueID);
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
