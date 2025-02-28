using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothing = 5f;
    private Vector3 offset;
    private float cameraHalfWidth;
    private float initialCameraX;
    public float minX;// Left border
    public float maxX; // Right border

    void Start()
    {
        offset = transform.position - player.position;

        // Store the initial camera position (so we don't move beyond it).
        initialCameraX = transform.position.x;

        // Calculate camera's half-width based on its orthographic size and aspect ratio.
        Camera cam = Camera.main;
        cameraHalfWidth = cam.orthographicSize * cam.aspect;
    }

    void FixedUpdate()
    {
        Vector3 targetPos = player.position + offset;

        // Only move right if player reaches the right edge of the camera's view
        if (player.position.x > transform.position.x + cameraHalfWidth)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);
        }
        // Only move left if player reaches the left edge of the camera's view
        else if (player.position.x < transform.position.x - cameraHalfWidth)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);
        }
    }
}