using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f;           // For vertical following (optional)
    public Transform target;                 // Target for vertical follow (if needed)
    public Transform leftWall;               // Assign the left wall in the Inspector
    public Transform rightWall;              // Assign the right wall in the Inspector
    public float fixedYPosition = 2f;        // Base y-position for the camera
    public float verticalOffset = 1f;        // Additional vertical offset

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        // Initial update in case the resolution is already set.
        UpdateCamera();
    }

    void Update()
    {
        // Recalculate camera settings every frame so that changes in aspect ratio (e.g., fullscreen)
        // keep the walls exactly at the left/right edges.
        UpdateCamera();

        // Optional vertical follow for the target.
        float newY = Mathf.Lerp(transform.position.y, target.position.y + verticalOffset, FollowSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, -10f);
    }

    void UpdateCamera()
    {
        // Calculate the horizontal center between the walls.
        float centerX = (leftWall.position.x + rightWall.position.x) / 2f;

        // Calculate half the horizontal distance between the walls.
        float halfDistance = (rightWall.position.x - leftWall.position.x) / 2f;

        // Set the orthographic size so the horizontal view exactly spans the walls.
        // (Horizontal half-width = cam.orthographicSize * cam.aspect)
        cam.orthographicSize = halfDistance / cam.aspect;

        // Set the camera's horizontal position to center the walls.
        // The vertical position is set to fixedYPosition + verticalOffset (and is updated by vertical follow).
        transform.position = new Vector3(centerX, transform.position.y, -10f);
    }
}
