using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public int mazeWidth;
    public int mazeHeight;
    public float cellSize;
    public int[] mazeData; // Serialized maze layout (e.g., WallState enum values)
    public Vector3 playerPosition;
    public Vector3 exitPosition;
    public List<Vector3> coinPositions; // List of coin positions
}

