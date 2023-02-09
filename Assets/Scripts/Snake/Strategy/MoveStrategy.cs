using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snake.Strategy
{
public abstract class MoveStrategy
{
    public abstract Vector3Int GetDirection(SnakeParts parts, BoundsInt bounds, Vector3Int apple);
    
    /// <summary>
    /// Returns a 2d array with true on a coordinate if occupied
    /// </summary>
    protected static Dictionary<Vector3Int, bool> GetGrid(SnakeParts parts, BoundsInt bounds)
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

    protected static LinkedList<Vector3Int> GetPossibleDirections(Dictionary<Vector3Int, bool> grid, Vector3Int pos)
    {
        var directions = new LinkedList<Vector3Int>(
            new []{Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right});

        return new LinkedList<Vector3Int>(directions.Where(
            direction => !grid[pos + direction]));
    }

    protected static int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }
}
}