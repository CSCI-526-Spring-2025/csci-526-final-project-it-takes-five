using UnityEngine;

public class NoFriction : MonoBehaviour
{
    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            PhysicsMaterial2D noFriction = new PhysicsMaterial2D();
            noFriction.friction = 0f;
            noFriction.bounciness = 0f; // Adjust if needed
            col.sharedMaterial = noFriction;
        }
    }
}