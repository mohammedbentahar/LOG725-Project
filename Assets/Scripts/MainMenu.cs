using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Charge la scène principale
    public void PlayGame()
    {
        SceneManager.LoadScene("Maze"); 
    }

    // Quitte l'application
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Le jeu a été quitté."); 
    }
}
