using System;
using System.Collections.Generic;
using System.Linq;
using Snake;
using UnityEngine;

namespace Utils
{
public static class Util
{
    public static Vector3Int QuaternionToDirection(Quaternion quaternion)
    {
        // Default direction is to the right
        return quaternion.eulerAngles.z switch
        {
            0 => Vector3Int.right,
            90 => Vector3Int.up,
            180 => Vector3Int.left,
            270 => Vector3Int.down,
            _ => throw new ArgumentException("must be one of the four primary angles", nameof(quaternion))
        };
    }

    public static Quaternion DirectionToQuaternion(Vector3Int direction)
    {
        // Default direction is to the right
        return direction switch
        {
            var v when v.Equals(Vector3Int.right) => Quaternion.Euler(0, 0, 0),
            var v when v.Equals(Vector3Int.up) => Quaternion.Euler(0, 0, 90),
            var v when v.Equals(Vector3Int.left) => Quaternion.Euler(0, 0, 180),
            var v when v.Equals(Vector3Int.down) => Quaternion.Euler(0, 0, 270),
            _ => throw new ArgumentException("must be one of the four primary directions", nameof(direction))
        };
    }
    
    public static int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }
    
    /// <summary>
    /// Returns a 2d array with true on a coordinate if occupied
    /// </summary>
    public static Dictionary<Vector3Int, bool> GetGrid(SnakeParts parts, BoundsInt bounds)
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
            var bla = part.Pos;
            grid[part.Pos] = true;
        }

        return grid;
    }

    public static LinkedList<Vector3Int> GetPossibleDirections(SnakeParts parts, BoundsInt bounds, Vector3Int pos)
    {
        return GetPossibleDirections(GetGrid(parts, bounds), pos);
    }

    public static LinkedList<Vector3Int> GetPossibleDirections(Dictionary<Vector3Int, bool> grid, Vector3Int pos)
    {
        var directions = new LinkedList<Vector3Int>(
            new []{Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right});

        return new LinkedList<Vector3Int>(directions.Where(
            direction => !grid[pos + direction]));
    }
}
}