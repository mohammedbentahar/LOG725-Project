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


    /// <summary>
    /// Finds the farthest edge position from the starting point (0, 0) using BFS.
    /// </summary>
    /// <param name="maze">Maze matrix</param>
    /// <param name="width">Maze width</param>
    /// <param name="height">Maze height</param>
    /// <returns>Position of the farthest edge cell</returns>
    public static Position FindFarthestEdgePosition(WallState[,] maze, int width, int height)
    {
        Position farthestPosition = new Position { X = 0, Y = 0 };
        int maxDistance = 0;

        // BFS queue to track cells and their distances
        Queue<(Position, int)> queue = new Queue<(Position, int)>();
        queue.Enqueue((new Position { X = 0, Y = 0 }, 0)); // Start from (0, 0)

        // HashSet to track visited cells
        HashSet<(int, int)> visited = new HashSet<(int, int)>();
        visited.Add((0, 0));

        // Directions for neighbor traversal (UP, DOWN, LEFT, RIGHT)
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { 1, -1, 0, 0 };
        WallState[] walls = { WallState.UP, WallState.DOWN, WallState.LEFT, WallState.RIGHT };

        while (queue.Count > 0)
        {
            var (current, distance) = queue.Dequeue();

            // Update farthest position if a longer distance is found and it's on the edge
            if (distance > maxDistance && IsEdgeCell(current, width, height))
            {
                maxDistance = distance;
                farthestPosition = current;
            }

            // Explore neighbors
            for (int i = 0; i < 4; i++)
            {
                int nx = current.X + dx[i];
                int ny = current.Y + dy[i];

                // Ensure neighbor is within bounds, not visited, and accessible
                if (nx >= 0 && nx < width && ny >= 0 && ny < height &&
                    !visited.Contains((nx, ny)) &&
                    !maze[current.X, current.Y].HasFlag(walls[i]))
                {
                    visited.Add((nx, ny));
                    queue.Enqueue((new Position { X = nx, Y = ny }, distance + 1));
                }
            }
        }

        return farthestPosition;
    }

    /// <summary>
    /// Checks if a cell is on the edge of the maze.
    /// </summary>
    /// <param name="pos">Cell position</param>
    /// <param name="width">Maze width</param>
    /// <param name="height">Maze height</param>
    /// <returns>True if the cell is on the edge, otherwise false</returns>
    private static bool IsEdgeCell(Position pos, int width, int height)
    {
        return pos.X == 0 || pos.X == width - 1 || pos.Y == 0 || pos.Y == height - 1;
    }





}
