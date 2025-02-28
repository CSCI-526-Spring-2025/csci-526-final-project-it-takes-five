using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public Color ghostColor = Color.white;
    public Color humanColor = Color.yellow;
    private bool isGhost = true;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        // Start as ghost (floating)
        rb.gravityScale = 0;
        sr.color = ghostColor;
    }

    void Update()
    {
        // Movement using A/D or left/right arrow keys
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    // Call this to transform the player from ghost to human.
    public void TransformToHuman()
    {
        if (isGhost)
        {
            isGhost = false;
            // Enable gravity so the player stops floating.
            rb.gravityScale = 1;
            // Change the sprite color to indicate transformation.
            sr.color = humanColor;
            Debug.Log("Player transformed to human!");
        }
    }
}
