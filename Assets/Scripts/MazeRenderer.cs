using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe responsable du rendu du labyrinthe
/// </summary>
public class MazeRenderer : MonoBehaviour
{
    [SerializeField, Range(1, 50)] private int _width = 10;
    [SerializeField, Range(1, 50)] private int _height = 10;
    [SerializeField] private float _cellSize = 1f;

    [SerializeField] private Transform _wallPrefab = null;
    [SerializeField] private Transform _floorPrefab = null;
    [SerializeField] private Transform _coinPrefab = null;
    [SerializeField] private Transform _exitPrefab = null;
    [SerializeField] private LineRenderer _lineRendererPrefab = null;

    [SerializeField, Range(1, 20)] private int _coinCount = 5;

    private Transform _player;
    private Vector3 _exitPosition;
    private Vector3 _lastPlayerPosition;
    private float _helpTimer = 30f; // Time interval to check player progress
    private bool _isPathShown = false;

    /// <summary>
    /// Méthode appelée au démarrage du script
    /// </summary>
    private void Start()
    {
        _player = GameObject.FindWithTag("Player")?.transform;

        WallState[,] maze = MazeGenerator.Generate(_width, _height);
        PlaceExit(maze);    // Placer la sortie avant de rendre le labyrinthe
        RenderMaze(maze);   // Rendre le labyrinthe après avoir placé la sortie
        PlaceCoins(maze);

        _lastPlayerPosition = _player.position;
        StartCoroutine(CheckPlayerProgress(maze));
    }

    /// <summary>
    /// Méthode responsable du rendu du labyrinthe
    /// </summary>
    private void RenderMaze(WallState[,] maze)
    {
        RenderFloor();

        for (int x = 0; x < _width; ++x)
        {
            for (int z = 0; z < _height; ++z)
            {
                WallState cell = maze[x, z];
                Vector3 position = CalculateCellPosition(x, z);

                RenderWallIfExists(cell, WallState.UP, position + new Vector3(0, 0, _cellSize / 2));
                RenderWallIfExists(cell, WallState.LEFT, position + new Vector3(-_cellSize / 2, 0, 0), 90);

                if (x == _width - 1)
                {
                    RenderWallIfExists(cell, WallState.RIGHT, position + new Vector3(_cellSize / 2, 0, 0), 90);
                }

                if (z == 0)
                {
                    RenderWallIfExists(cell, WallState.DOWN, position + new Vector3(0, 0, -_cellSize / 2));
                }
            }
        }
    }

    private void RenderFloor()
    {
        Transform floor = Instantiate(_floorPrefab, transform);
        floor.localScale = new Vector3(_width, 1, _height);
    }

    private Vector3 CalculateCellPosition(int x, int z)
    {
        return new Vector3(-_width / 2 + x, 0, -_height / 2 + z);
    }

    private void RenderWallIfExists(WallState cell, WallState wallState, Vector3 position, float rotationY = 0)
    {
        if (cell.HasFlag(wallState))
        {
            Transform wall = Instantiate(_wallPrefab, transform);
            wall.position = position;
            wall.localScale = new Vector3(_cellSize, wall.localScale.y, wall.localScale.z);
            wall.eulerAngles = new Vector3(0, rotationY, 0);
        }
    }

    private void PlaceCoins(WallState[,] maze)
    {
        if (_coinPrefab == null)
        {
            Debug.LogError("Le prefab de pièces n'est pas assigné dans l'inspecteur !");
            return;
        }

        var rng = new System.Random();
        var placedPositions = new HashSet<Vector3>();

        for (int i = 0; i < _coinCount; ++i)
        {
            int x, z;
            Vector3 position;

            do
            {
                x = rng.Next(0, _width);
                z = rng.Next(0, _height);
                position = CalculateCellPosition(x, z);
            } while (placedPositions.Contains(position));

            placedPositions.Add(position);
            Transform coin = Instantiate(_coinPrefab, transform);
            coin.position = position + new Vector3(0, 0.5f, 0);
        }
    }

    private void PlaceExit(WallState[,] maze)
    {
        if (_exitPrefab == null)
        {
            Debug.LogError("Exit prefab is not assigned in the Inspector!");
            return;
        }

        // Find the farthest edge cell
        Position farthestCell = MazeGenerator.FindFarthestEdgePosition(maze, _width, _height);

        // Calculate the exit position in world coordinates
        _exitPosition = CalculateCellPosition(farthestCell.X, farthestCell.Y);
        Quaternion exitRotation = Quaternion.identity;

        // Adjust the wall and position for the exit
        if (farthestCell.X == _width - 1) // Right edge
        {
            maze[farthestCell.X, farthestCell.Y] &= ~WallState.RIGHT;
            _exitPosition += new Vector3(_cellSize / 2, 0, 0);
            exitRotation = Quaternion.Euler(0, 90, 0);
        }
        else if (farthestCell.Y == 0) // Bottom edge
        {
            maze[farthestCell.X, farthestCell.Y] &= ~WallState.DOWN;
            _exitPosition += new Vector3(0, 0, -_cellSize / 2);
        }
        else if (farthestCell.X == 0) // Left edge
        {
            maze[farthestCell.X, farthestCell.Y] &= ~WallState.LEFT;
            _exitPosition += new Vector3(-_cellSize / 2, 0, 0);
            exitRotation = Quaternion.Euler(0, 90, 0);
        }
        else if (farthestCell.Y == _height - 1) // Top edge
        {
            maze[farthestCell.X, farthestCell.Y] &= ~WallState.UP;
            _exitPosition += new Vector3(0, 0, _cellSize / 2);
        }

        // Instantiate the exit prefab
        Instantiate(_exitPrefab, _exitPosition + new Vector3(0, 0.5f, 0), exitRotation);

        Debug.Log($"Exit placed at: {farthestCell.X}, {farthestCell.Y}");
    }


    /// <summary>
    /// Vérifie la progression du joueur et affiche un chemin s'il est bloqué.
    /// </summary>
    private IEnumerator CheckPlayerProgress(WallState[,] maze)
    {
        while (true)
        {
            yield return new WaitForSeconds(_helpTimer);

            if (_player == null) yield break;

            float distanceMoved = Vector3.Distance(_player.position, _lastPlayerPosition);
            if (distanceMoved < _cellSize) // If player hasn't moved significantly
            {
                if (!_isPathShown)
                {
                    ShowHelpPath(maze);
                    yield return new WaitForSeconds(5f); // Show path for 5 seconds
                    ClearHelpPath();
                }
            }

            _lastPlayerPosition = _player.position;
        }
    }

    /// <summary>
    /// Affiche un chemin vers la sortie.
    /// </summary>
    private void ShowHelpPath(WallState[,] maze)
    {
        if (_lineRendererPrefab == null)
        {
            Debug.LogError("LineRenderer prefab is not assigned in the Inspector!");
            return;
        }

        Position playerCell = new Position
        {
            X = Mathf.Clamp(Mathf.FloorToInt(_player.position.x + _width / 2), 0, _width - 1),
            Y = Mathf.Clamp(Mathf.FloorToInt(_player.position.z + _height / 2), 0, _height - 1)
        };

        Position exitCell = new Position
        {
            X = Mathf.Clamp(Mathf.FloorToInt(_exitPosition.x + _width / 2), 0, _width - 1),
            Y = Mathf.Clamp(Mathf.FloorToInt(_exitPosition.z + _height / 2), 0, _height - 1)
        };

        Debug.Log($"Player cell: {playerCell.X}, {playerCell.Y}");
        Debug.Log($"Exit cell: {exitCell.X}, {exitCell.Y}");

        List<Position> path = MazeGenerator.CalculateShortestPath(maze, playerCell, exitCell, _width, _height);
        if (path == null || path.Count == 0)
        {
            Debug.LogError("No valid path found!");
            return;
        }

        LineRenderer lineRenderer = Instantiate(_lineRendererPrefab, transform);

        Vector3[] positions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            positions[i] = CalculateCellPosition(path[i].X, path[i].Y) + new Vector3(0, 0.5f, 0);
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }


    private void ClearHelpPath()
    {
        _isPathShown = false;
        foreach (var line in FindObjectsOfType<LineRenderer>())
        {
            Destroy(line.gameObject);
        }
    }

    private void Update()
    {
        Transform player = GameObject.FindWithTag("Player")?.transform;
        if (player != null && Vector3.Distance(player.position, _exitPosition) < 1f)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("Félicitations ! Vous avez atteint la sortie !");
    }
}
