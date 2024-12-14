using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;  // Reference to the player's Transform
    public float distance = 0.0f;  // Distance behind the player
    public float height = 0f;  // Height of the camera above the player
    public float rotationSpeed = 1.0f;  // Speed of camera rotation
    public float smoothSpeed = 10.0f;  // Smoothness of camera movement

    private float currentRotationY = 0f;  // Current Y rotation
    private float currentPitch = 10f;  // Current X rotation (camera pitch)

    public LayerMask collisionLayers;  // Define which layers the camera should check for collisions
    public float minDistance = 1.0f;  // Minimum distance from the player
    public float cameraRadius = 0.2f; // Radius for collision detection (to simulate camera size)

    void Update()
    {
        // Handle mouse input for camera rotation (same as before)
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        currentRotationY += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, -30f, 60f);

        Quaternion rotation = Quaternion.Euler(currentPitch, currentRotationY, 0);
        Vector3 desiredOffset = rotation * new Vector3(0, height, -distance);
        Vector3 desiredPosition = player.position + desiredOffset;

        // Collision detection using Physics.Raycast
        Ray ray = new Ray(player.position, desiredOffset.normalized);
        if (Physics.SphereCast(ray, cameraRadius, out RaycastHit hit, distance, collisionLayers))
        {
            // Adjust the camera position to be just before the obstacle
            desiredPosition = hit.point - desiredOffset.normalized * 0.1f; // Offset slightly to avoid clipping
        }

        // Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        // Make the camera look at the player
        transform.LookAt(player.position + Vector3.up * 1.0f);
    }

}
