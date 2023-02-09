using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeAI
{
public abstract class MoveStrategy
{
    /// <summary>
    /// Returns a 2d array with true on a coordinate if occupied
    /// </summary>
    protected Dictionary<Vector3Int, bool> GetGrid(LinkedList<SnakePart> parts, BoundsInt bounds)
    {
        var grid = new Dictionary<Vector3Int, bool>();

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                if (x == bounds.xMin || y == bounds.yMin || x == bounds.xMax - 1 || y == bounds.yMax - 1)
                    grid[new Vector3Int(x, y)] = true;
                else
                    grid[new Vector3Int(x, y)] = false;
            }
        }

        foreach (SnakePart part in parts)
        {
            grid[part.Pos] = true;
        }

        return grid;
    }

    protected LinkedList<Vector3Int> GetPossibleDirections(Dictionary<Vector3Int, bool> grid, Vector3Int pos)
    {
        var directions = new LinkedList<Vector3Int>(
            new []{Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right});

        var result = new LinkedList<Vector3Int>();

        foreach (Vector3Int direction in directions)
        {
            if (!grid[pos + direction])
                result.AddLast(direction);
        }

        return result;
    }

    protected static int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    public abstract Vector3Int GetDirection(LinkedList<SnakePart> parts, BoundsInt bounds, Vector3Int apple);
}
}