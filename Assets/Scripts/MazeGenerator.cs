using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Définition des états des murs dans le labyrinthe
/// </summary>
[Flags]
public enum WallState
{
    // Aucun mur
    NONE = 0, // 0000

    // Murs individuels
    LEFT = 1,   // 0001
    RIGHT = 2,  // 0010
    UP = 4,     // 0100
    DOWN = 8,   // 1000

    // Indique qu'une cellule a été visitée
    VISITED = 128 // 1000 0000
}

/// <summary>
/// Structure pour représenter une position dans la grille
/// </summary>
public struct Position
{
    public int X;
    public int Y;
}

/// <summary>
/// Structure pour représenter un voisin avec un mur partagé
/// </summary>
public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}

/// <summary>
/// Générateur de labyrinthes utilisant l'algorithme de backtracking récursif
/// </summary>
public static class MazeGenerator
{
    /// <summary>
    /// Retourne le mur opposé pour un mur donné
    /// </summary>
    /// <param name="wall">Mur dont on veut l'opposé</param>
    /// <returns>Mur opposé</returns>
    private static WallState GetOppositeWall(WallState wall)
    {
        return wall switch
        {
            WallState.RIGHT => WallState.LEFT,
            WallState.LEFT => WallState.RIGHT,
            WallState.UP => WallState.DOWN,
            WallState.DOWN => WallState.UP,
            _ => WallState.NONE
        };
    }

    /// <summary>
    /// Applique l'algorithme de backtracking récursif pour générer un labyrinthe
    /// </summary>
    /// <param name="maze">Matrice représentant le labyrinthe</param>
    /// <param name="width">Largeur de la grille</param>
    /// <param name="height">Hauteur de la grille</param>
    /// <returns>Labyrinthe généré</returns>
    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height)
    {
        var rng = new System.Random(); // Générateur aléatoire
        var positionStack = new Stack<Position>();
        var initialPosition = new Position
        {
            X = rng.Next(0, width),
            Y = rng.Next(0, height)
        };

        maze[initialPosition.X, initialPosition.Y] |= WallState.VISITED;
        positionStack.Push(initialPosition);

        while (positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count > 0)
            {
                positionStack.Push(current);

                // Sélection d'un voisin aléatoire
                var randomNeighbour = neighbours[rng.Next(neighbours.Count)];

                // Mise à jour des murs entre les cellules
                maze[current.X, current.Y] &= ~randomNeighbour.SharedWall;
                maze[randomNeighbour.Position.X, randomNeighbour.Position.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall);
                maze[randomNeighbour.Position.X, randomNeighbour.Position.Y] |= WallState.VISITED;

                positionStack.Push(randomNeighbour.Position);
            }
        }

        return maze;
    }

    /// <summary>
    /// Récupère les voisins non visités d'une position donnée
    /// </summary>
    /// <param name="p">Position actuelle</param>
    /// <param name="maze">Matrice du labyrinthe</param>
    /// <param name="width">Largeur de la grille</param>
    /// <param name="height">Hauteur de la grille</param>
    /// <returns>Liste des voisins non visités</returns>
    private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, int width, int height)
    {
        var neighbours = new List<Neighbour>();

        // Vérifie le voisin à gauche
        if (p.X > 0 && !maze[p.X - 1, p.Y].HasFlag(WallState.VISITED))
        {
            neighbours.Add(new Neighbour
            {
                Position = new Position { X = p.X - 1, Y = p.Y },
                SharedWall = WallState.LEFT
            });
        }

        // Vérifie le voisin en bas
        if (p.Y > 0 && !maze[p.X, p.Y - 1].HasFlag(WallState.VISITED))
        {
            neighbours.Add(new Neighbour
            {
                Position = new Position { X = p.X, Y = p.Y - 1 },
                SharedWall = WallState.DOWN
            });
        }

        // Vérifie le voisin en haut
        if (p.Y < height - 1 && !maze[p.X, p.Y + 1].HasFlag(WallState.VISITED))
        {
            neighbours.Add(new Neighbour
            {
                Position = new Position { X = p.X, Y = p.Y + 1 },
                SharedWall = WallState.UP
            });
        }

        // Vérifie le voisin à droite
        if (p.X < width - 1 && !maze[p.X + 1, p.Y].HasFlag(WallState.VISITED))
        {
            neighbours.Add(new Neighbour
            {
                Position = new Position { X = p.X + 1, Y = p.Y },
                SharedWall = WallState.RIGHT
            });
        }

        return neighbours;
    }

    /// <summary>
    /// Génère un labyrinthe avec des murs initiaux partout
    /// </summary>
    /// <param name="width">Largeur de la grille</param>
    /// <param name="height">Hauteur de la grille</param>
    /// <returns>Labyrinthe généré</returns>
    public static WallState[,] Generate(int width, int height)
    {
        var maze = new WallState[width, height];
        WallState initialWallState = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;

        // Initialisation des murs
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                maze[x, y] = initialWallState;
            }
        }

        return ApplyRecursiveBacktracker(maze, width, height);
    }
}
