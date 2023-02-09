using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snake.Strategy
{
public class SimpleStrategy : MoveStrategy
{
    public override Vector3Int GetDirection(SnakeParts parts, BoundsInt bounds, Vector3Int apple)
    {
        Vector3Int headPos = parts.Last.Value.Pos;
        
        Dictionary<Vector3Int, bool> grid = GetGrid(parts, bounds);
        LinkedList<Vector3Int> directions = GetPossibleDirections(grid, headPos);

        return directions
            .DefaultIfEmpty(Vector3Int.zero)
            .Select(direction => new Tuple<Vector3Int, int>(direction,
                ManhattanDistance((headPos + direction), apple)))
            .OrderBy(tuple => tuple.Item2)
            .First()
            .Item1;
    }
}
}