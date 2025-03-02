using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public Color ghostColor = Color.white;
    public Color humanColor = Color.yellow;

    private bool isGhost = true;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // Orb stack: capacity is 3.
    private Stack<GameObject> orbStack = new Stack<GameObject>();
    public int orbStackCapacity = 3;

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
    public GameObject[] orbSlots;  // Assign 3 UI Image slot GameObjects in the Inspector
    public Sprite emptySlotSprite; // Sprite to use for an empty slot

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

    void Update()
    {
        // Movement using A/D or left/right arrow keys.
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // Pick up orbs when pressing F.
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (nearbyOrb != null && orbStack.Count < orbStackCapacity)
            {
                orbStack.Push(nearbyOrb);
                nearbyOrb.SetActive(false); // Deactivate the orb.
                Debug.Log("Picked up orb. Orb count: " + orbStack.Count);
                UpdateOrbUI();
                nearbyOrb = null; // Clear reference after pickup.
            }
        }

        // Jump in human mode (when not a ghost) using W.
        if (!isGhost && Input.GetKeyDown(KeyCode.W))
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
        }

        // Press SPACE to drop the top orb at the player's position.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (orbStack.Count > 0)
            {
                GameObject topOrb = orbStack.Pop();
                topOrb.transform.position = transform.position;
                topOrb.SetActive(true);
                UpdateOrbUI();
            }
        }
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
                Debug.Log("Orb in pickup range. Press F to pick it up.");
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
                slotImage.sprite = emptySlotSprite;
                slotImage.color = Color.white;
            }
        }
    }

}
