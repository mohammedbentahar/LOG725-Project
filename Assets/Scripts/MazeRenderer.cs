using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe responsable du rendu du labyrinthe
/// </summary>
public class MazeRenderer : MonoBehaviour
{
    // Largeur du labyrinthe (paramètre modifiable dans l'inspecteur)
    [SerializeField, Range(1, 50)] private int _width = 10;

    // Hauteur du labyrinthe (paramètre modifiable dans l'inspecteur)
    [SerializeField, Range(1, 50)] private int _height = 10;

    // Taille des cellules et des murs
    [SerializeField] private float _cellSize = 1f;

    // Préfabriqué pour les murs
    [SerializeField] private Transform _wallPrefab = null;

    // Préfabriqué pour le sol
    [SerializeField] private Transform _floorPrefab = null;

    /// <summary>
    /// Méthode appelée au démarrage du script
    /// </summary>
    private void Start()
    {
        // Génération du labyrinthe
        WallState[,] maze = MazeGenerator.Generate(_width, _height);

        // Rendu du labyrinthe
        RenderMaze(maze);
    }

    /// <summary>
    /// Méthode responsable du rendu du labyrinthe
    /// </summary>
    /// <param name="maze">Matrice représentant l'état des murs du labyrinthe</param>
    private void RenderMaze(WallState[,] maze)
    {
        // Instanciation du sol
        RenderFloor();

        // Parcours des cellules du labyrinthe pour dessiner les murs
        for (int x = 0; x < _width; ++x)
        {
            for (int z = 0; z < _height; ++z)
            {
                WallState cell = maze[x, z];
                Vector3 position = CalculateCellPosition(x, z);

                // Dessin des murs si nécessaire
                RenderWallIfExists(cell, WallState.UP, position + new Vector3(0, 0, _cellSize / 2));
                RenderWallIfExists(cell, WallState.LEFT, position + new Vector3(-_cellSize / 2, 0, 0), 90);

                // Dessin des murs de bordure (droite et bas)
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
    /// Méthode pour instancier le sol du labyrinthe
    /// </summary>
    private void RenderFloor()
    {
        Transform floor = Instantiate(_floorPrefab, transform);
        floor.localScale = new Vector3(_width, 1, _height);
    }

    /// <summary>
    /// Méthode pour calculer la position d'une cellule
    /// </summary>
    /// <param name="x">Indice X de la cellule</param>
    /// <param name="z">Indice Z de la cellule</param>
    /// <returns>Position dans le monde réel de la cellule</returns>
    private Vector3 CalculateCellPosition(int x, int z)
    {
        return new Vector3(-_width / 2 + x, 0, -_height / 2 + z);
    }

    /// <summary>
    /// Méthode pour instancier un mur si le mur est requis
    /// </summary>
    /// <param name="cell">État des murs de la cellule</param>
    /// <param name="wallState">Direction du mur à vérifier</param>
    /// <param name="position">Position du mur</param>
    /// <param name="rotationY">Rotation Y du mur (par défaut 0)</param>
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
}
