using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
    private string saveFilePath;

    private void Start()
    {
        // Define the path to the save file
        saveFilePath = Application.persistentDataPath + "/savegame.json";

        // Disable the Continue button if no save file exists
        GameObject continueButton = GameObject.Find("ContinueButton");
        if (continueButton != null)
        {
            continueButton.SetActive(File.Exists(saveFilePath));
        }
    }

    // Start a new game by loading the Maze scene
    public void PlayGame()
    {
        SceneManager.LoadScene("Maze");
    }

    // Load the saved game state
    public void ContinueGame()
    {
        if (File.Exists(saveFilePath))
        {
            // Read the save file
            string json = File.ReadAllText(saveFilePath);

            // Deserialize the save data
            PauseMenuManager.GameState savedState = JsonUtility.FromJson<PauseMenuManager.GameState>(json);

            // Load the saved scene
            SceneManager.LoadScene(savedState.currentSceneIndex);

            // Use Unity's built-in method to execute code after scene loads
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                // Restore the player's position
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    player.transform.position = savedState.playerPosition;
                }
            };

            Debug.Log("Game Loaded!");
        }
        else
        {
            Debug.LogWarning("No save file found!");
        }
    }

    // Quit the game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Le jeu a été quitté.");
    }
}
