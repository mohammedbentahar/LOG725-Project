using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour
{
    public void RestartGame()
    {
        Debug.Log("Restart button clicked!");
        SceneManager.LoadScene("Maze");
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Return to Main Menu button clicked!");
        SceneManager.LoadScene("MainMenu");
    }

}
