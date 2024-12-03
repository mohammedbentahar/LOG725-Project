using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Camera miniMapCamera; // Reference to the mini-map camera
    public RawImage miniMapImage; // Image to display the mini-map
    public RectTransform miniMapRect; // For positioning the mini-map on the screen

    void Start()
    {
        // Ensure the mini-map camera is properly set up
        if (miniMapCamera == null)
        {
            Debug.LogError("MiniMapCamera is not assigned in the Inspector!");
            return;
        }

        // Create and assign a render texture for the mini-map
        miniMapCamera.targetTexture = new RenderTexture(256, 256, 16); // Create a texture for the mini-map
        miniMapImage.texture = miniMapCamera.targetTexture; // Assign the texture to the mini-map image

        // Adjust the position and size of the mini-map on the screen
        miniMapRect.anchorMin = new Vector2(1, 0); // Anchor at the bottom-right corner
        miniMapRect.anchorMax = new Vector2(1, 0);
        miniMapRect.pivot = new Vector2(1, 0);
        miniMapRect.sizeDelta = new Vector2(200, 200); // Mini-map size on the screen

        // Adjust the camera's orthographic size based on difficulty level
        AdjustMiniMapSize();
    }

    private void AdjustMiniMapSize()
    {
        if (DifficultyManager.SelectedDifficulty == 0) // Beginner
        {
            miniMapCamera.orthographicSize = 6f;
        }
        else if (DifficultyManager.SelectedDifficulty == 1) // Intermediate
        {
            miniMapCamera.orthographicSize = 12f;
        }
        else if (DifficultyManager.SelectedDifficulty == 2) // Advanced
        {
            miniMapCamera.orthographicSize = 18f;
        }
        else
        {
            Debug.LogWarning("Invalid difficulty level detected! Defaulting to beginner size.");
            miniMapCamera.orthographicSize = 6f; // Default to beginner
        }

        Debug.Log($"MiniMapCamera size set to {miniMapCamera.orthographicSize} based on difficulty.");
    }
}
