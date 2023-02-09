using System.Collections.Generic;
using UnityEngine;

namespace SnakeAI
{
public class SimpleStrategy : MoveStrategy
{
    public override Vector3Int GetDirection(LinkedList<SnakePart> parts, BoundsInt bounds, Vector3Int apple)
    {
        Vector3Int headPos = parts.Last.Value.Pos;
        
        Dictionary<Vector3Int, bool> grid = GetGrid(parts, bounds);
        LinkedList<Vector3Int> directions = GetPossibleDirections(grid, headPos);

        int curDistance = ManhattanDistance(headPos, apple);
        foreach (Vector3Int direction in directions)
        {
            if (ManhattanDistance((headPos + direction), apple) < curDistance)
                return direction;
        }

        foreach (Vector3Int direction in directions)
        {
            if (ManhattanDistance((headPos + direction), apple) == curDistance)
                return direction;
        }

        return directions.Count == 0 ? new Vector3Int() : directions.First.Value;
    }
}
}