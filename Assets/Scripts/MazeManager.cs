using UnityEngine;

public class MazeManager : MonoBehaviour
{
    [SerializeField] private Transform _exitPrefab; // Prefab for the exit
    [SerializeField] private Transform _player; // Player reference
    private Vector3 _exitPosition; // World position of the exit

    public void RenderMazeWithExit(WallState[,] maze, int width, int height)
    {
        // Render the maze (existing rendering logic here)

        // Find the farthest position
        Position farthestPosition = MazeGenerator.FindFarthestPosition(maze, width, height);

        // Place the exit
        _exitPosition = new Vector3(farthestPosition.X - width / 2, 0, farthestPosition.Y - height / 2);
        Instantiate(_exitPrefab, _exitPosition, Quaternion.identity);
    }

    private void Update()
    {
        // Check if the player has reached the exit
        if (Vector3.Distance(_player.position, _exitPosition) < 1.0f)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("Congratulations! You reached the exit!");
        // Add additional end-game logic here (e.g., show UI, restart level)
    }
}
