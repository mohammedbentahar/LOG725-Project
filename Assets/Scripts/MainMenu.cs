using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Charge la sc�ne principale
    public void PlayGame()
    {
        SceneManager.LoadScene("Maze"); // Remplacez "GameScene" par le nom de votre sc�ne de jeu
    }

    // Quitte l'application
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Le jeu a �t� quitt�."); // Visible uniquement dans l'�diteur
    }
}
