using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Reference to the player’s Transform
    public float distance = 5.0f;  // Distance between camera and player
    public float height = 1.0f;  // Height of the camera relative to the player
    public float rotationSpeed = 5.0f;  // Speed at which the camera rotates around the player

    private float currentRotationAngle = 45f;  // Current rotation around the player

    void Update()
    {
        // Get the player's input for rotation (horizontal axis)
        currentRotationAngle += Input.GetAxis("Horizontal") * rotationSpeed;

        // Calculate the new camera position
        Vector3 desiredPosition = player.position - (Vector3.forward * distance);
        desiredPosition.y = player.position.y + height;  // Keep the camera above the player

        // Apply smooth transition to follow the player’s position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * rotationSpeed);

        // Make the camera look at the player
        transform.LookAt(player);
    }
}
