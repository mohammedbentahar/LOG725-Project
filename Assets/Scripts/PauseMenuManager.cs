using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; // Assign in the Inspector
    private bool isPaused = false;

    void Update()
    {
        // Detect ESC key press to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape Key Pressed");
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
        {
            Debug.Log("Resuming Game");
            pauseMenuUI.SetActive(false); // Hide the pause menu
            Time.timeScale = 1f; // Resume game time
            isPaused = false;
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null)
        {
            Debug.Log("Activating Pause Menu");
            pauseMenuUI.SetActive(true); // Show the pause menu
            Time.timeScale = 0f; // Pause game time
            isPaused = true;
        }
        else
        {
            Debug.LogWarning("pauseMenuUI is not assigned in the Inspector!");
        }
    }


    public void SaveGame()
    {
        Debug.Log("Game Saved! (Implement save logic here)");
        // Add save logic here
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu");
        Time.timeScale = 1f; // Ensure game time is running before switching scenes
        SceneManager.LoadScene("MainMenu");
    }
}
