using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; // Assign in the Inspector
    private bool isPaused = false;

    // Define a structure to hold game state
    [System.Serializable]
    public class GameState
    {
        public Vector3 playerPosition;
        public int currentSceneIndex;
        // Add other variables like maze state, score, etc., as needed
    }

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
        // Create a new game state object
        GameState state = new GameState();
        state.playerPosition = GameObject.FindWithTag("Player").transform.position;
        state.currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Serialize the game state to JSON
        string json = JsonUtility.ToJson(state);

        // Save the JSON string to a file
        string path = Application.persistentDataPath + "/savegame.json";
        File.WriteAllText(path, json);

        Debug.Log("Game Saved! Path: " + path);
    }


    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu");
        Time.timeScale = 1f; // Ensure game time is running before switching scenes
        SceneManager.LoadScene("MainMenu");
    }
}
