using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Drag the Player GameObject here.
    public float smoothing = 5f;
    private Vector3 offset;

    void Start()
    {
        // Calculate the initial offset.
        offset = transform.position - player.position;
    }

    void FixedUpdate()
    {
        Vector3 targetPos = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);
    }
}
