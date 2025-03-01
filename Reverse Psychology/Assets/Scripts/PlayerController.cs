using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f; // The jump force applied when using orbs.
    public Color ghostColor = Color.white;
    public Color humanColor = Color.yellow;
    
    private bool isGhost = true;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // Orb stack: capacity is 3.
    private Stack<GameObject> orbStack = new Stack<GameObject>();
    public int orbStackCapacity = 3;

    // UI element to display orb count (assign a Text element from your UI)
    public TextMeshProUGUI orbStackUIText;

    // Optional ground check settings to allow jumping only when grounded.
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private Collider2D col;
    private List<Collider2D> wallColliders = new List<Collider2D>();
    private GameObject nearbyOrb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Start as ghost (floating)
        gameObject.layer = LayerMask.NameToLayer("Ghost");
        rb.gravityScale = 0;
        sr.color = ghostColor;
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
        // Movement using A/D or left/right arrow keys
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if (isGhost && Input.GetKeyDown(KeyCode.F))
        {
            if (nearbyOrb != null && orbStack.Count < orbStackCapacity)
            {
                orbStack.Push(nearbyOrb);
                nearbyOrb.SetActive(false); // Deactivate the orb.
                Debug.Log("Picked up orb manually. Orb count: " + orbStack.Count);
                UpdateOrbUI();
                nearbyOrb = null; // Clear reference after pickup.
            }
        }

        if (!isGhost && Input.GetKeyDown(KeyCode.W))
        {
            if (orbStack.Count >= 2)
            {
                // Remove two orbs from the stack.
                orbStack.Pop();
                orbStack.Pop();
                UpdateOrbUI();

                // Apply upward jump force.
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                Debug.Log("Jump performed using 2 blue orbs.");
            }
        }
    }

    // private bool IsGrounded()
    // {
    //     return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    // }

    // Call this to transform the player from ghost to human.
    public void TransformToHuman()
    {
        if (isGhost)
        {
            isGhost = false;
            // Enable gravity so the player stops floating.
            rb.gravityScale = 1;
            foreach (Collider2D wallCol in wallColliders)
            {
                Physics2D.IgnoreCollision(col, wallCol, false);
            }
            // Switch layer to "Human" so wall collisions are enabled.
            gameObject.layer = LayerMask.NameToLayer("Human");
            // Change the sprite color to indicate transformation.
            sr.color = humanColor;
            Debug.Log("Player transformed to human!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BlueOrb") && isGhost)
        {
            // Set nearby orb if not already set.
            if (nearbyOrb == null)
            {
                nearbyOrb = collision.gameObject;
                Debug.Log("Orb in pickup range. Press F to pick it up.");
            }
        }
    
    }

    // When the player leaves the orb's trigger zone, clear the reference.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BlueOrb") && isGhost)
        {
            if (collision.gameObject == nearbyOrb)
            {
                nearbyOrb = null;
                Debug.Log("Orb left pickup range.");
            }
        }
    }

    // Update the UI Text to show the current orb count.
    private void UpdateOrbUI()
    {
        if (orbStackUIText != null)
        {
            orbStackUIText.text = "Orbs: " + orbStack.Count;
        }
    }
}
