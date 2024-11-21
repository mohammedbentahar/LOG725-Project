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

    [SerializeField, Range(1, 20)] private int _coinCount = 5;

    private Vector3 _exitPosition;

    /// <summary>
    /// Méthode appelée au démarrage du script
    /// </summary>
    private void Start()
    {
        WallState[,] maze = MazeGenerator.Generate(_width, _height);
        PlaceExit(maze);    // Placer la sortie avant de rendre le labyrinthe
        RenderMaze(maze);   // Rendre le labyrinthe après avoir placé la sortie
        PlaceCoins(maze);
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

    /// <summary>
    /// Méthode responsable du rendu du sol
    /// </summary>
    private void RenderFloor()
    {
        Transform floor = Instantiate(_floorPrefab, transform);
        floor.localScale = new Vector3(_width, 1, _height);
    }

    /// <summary>
    /// Calcule la position d'une cellule dans le labyrinthe
    /// </summary>
    private Vector3 CalculateCellPosition(int x, int z)
    {
        return new Vector3(-_width / 2 + x, 0, -_height / 2 + z);
    }

    /// <summary>
    /// Rend un mur si le mur spécifié existe dans la cellule
    /// </summary>
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

    /// <summary>
    /// Place les pièces dans le labyrinthe
    /// </summary>
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

    /// <summary>
    /// Place the exit at the farthest edge position and remove the corresponding wall.
    /// </summary>
    private void PlaceExit(WallState[,] maze)
    {
        if (_exitPrefab == null)
        {
            Debug.LogError("Le prefab de sortie n'est pas assigné dans l'inspecteur !");
            return;
        }

        // Trouver la position la plus éloignée sur le bord
        Position farthestCell = MazeGenerator.FindFarthestEdgePosition(maze, _width, _height);

        // Calculer la position en coordonnées du monde
        Vector3 exitPosition = CalculateCellPosition(farthestCell.X, farthestCell.Y);
        Quaternion exitRotation = Quaternion.identity;

        // Ajuster le mur et la position pour la sortie
        if (farthestCell.X == _width - 1) // Bord droit
        {
            maze[farthestCell.X, farthestCell.Y] &= ~WallState.RIGHT;
            exitPosition += new Vector3(_cellSize / 2, 0, 0);
            exitRotation = Quaternion.Euler(0, 90, 0);
        }
        else if (farthestCell.Y == 0) // Bord bas
        {
            maze[farthestCell.X, farthestCell.Y] &= ~WallState.DOWN;
            exitPosition += new Vector3(0, 0, -_cellSize / 2);
        }
        else if (farthestCell.X == 0) // Bord gauche
        {
            maze[farthestCell.X, farthestCell.Y] &= ~WallState.LEFT;
            exitPosition += new Vector3(-_cellSize / 2, 0, 0);
            exitRotation = Quaternion.Euler(0, 90, 0);
        }
        else if (farthestCell.Y == _height - 1) // Bord haut
        {
            maze[farthestCell.X, farthestCell.Y] &= ~WallState.UP;
            exitPosition += new Vector3(0, 0, _cellSize / 2);
        }

        _exitPosition = exitPosition;
        Instantiate(_exitPrefab, _exitPosition + new Vector3(0, 0.5f, 0), exitRotation);

        Debug.Log($"Sortie placée en : {farthestCell.X}, {farthestCell.Y}");
    }




    /// <summary>
    /// Vérifie si le joueur atteint la sortie
    /// </summary>
    private void Update()
    {
        Transform player = GameObject.FindWithTag("Player")?.transform;
        if (player != null && Vector3.Distance(player.position, _exitPosition) < 1f)
        {
            EndGame();
        }
    }

    /// <summary>
    /// Terminer la partie
    /// </summary>
    private void EndGame()
    {
        Debug.Log("Félicitations ! Vous avez atteint la sortie ! :) ");
        // Ajouter une logique supplémentaire pour la fin de la partie (UI, redémarrage, etc.)
    }
}
