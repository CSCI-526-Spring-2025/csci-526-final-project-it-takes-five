using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float followSpeed = 5f;       // Camera follow speed
    public Transform target;             // Player
    public Transform leftWall;           // Left boundary
    public Transform rightWall;          // Right boundary
    public float minOrthographicSize = 3f; // Minimum zoom-out size
    public float maxOrthographicSize = 6f; // Maximum zoom-out size
    public float verticalOffset = -4.05f;    // Additional vertical offset
    public float fixedYPosition = 5.51f;    // Fixed vertical position of the camera
    // public float levelSizeThreshold = 21.3f; // Hardcoded level size beyond which camera follows normally

    private Camera cam;
    private float minX, maxX;

    void Start()
    {
        cam = Camera.main;
        AdjustCameraSize();  // Adjust camera size based on level size
        UpdateCameraBounds();
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    void AdjustCameraSize()
    {
        // Calculate the required orthographic size based on the level width
        float levelWidth = rightWall.position.x - leftWall.position.x;
        float screenRatio = (float)Screen.width / Screen.height;

        float requiredSize = levelWidth / (2f * screenRatio); // Half width divided by aspect ratio

        // Clamp between min and max zoom-out values
        cam.orthographicSize = Mathf.Clamp(requiredSize, minOrthographicSize, maxOrthographicSize);
    }

    void UpdateCameraBounds()
    {
        float halfWidth = cam.orthographicSize * cam.aspect;
        minX = leftWall.position.x + halfWidth;
        maxX = rightWall.position.x - halfWidth;
    }

    void UpdateCameraPosition()
    {
        if (target == null) return;

        float targetX = Mathf.Lerp(transform.position.x, target.position.x, followSpeed * Time.deltaTime);
        float clampedX = Mathf.Clamp(targetX, minX, maxX);

        transform.position = new Vector3(clampedX, fixedYPosition + verticalOffset, -10f);
    }
}
