﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Snake.Strategy
{
public class SimpleStrategy : MoveStrategy
{
    private BoundsInt _bounds;

    public SimpleStrategy(BoundsInt bounds)
    {
        _bounds = bounds;
    }
    
    public Vector3Int GetDirection(SnakeParts parts, Vector3Int target)
    {
        Vector3Int headPos = parts.Last.Value.Pos;
        
        LinkedList<Vector3Int> directions = Util.GetPossibleDirections(parts, _bounds, headPos);

        return directions
            .DefaultIfEmpty(Vector3Int.zero)
            .Select(direction => new Tuple<Vector3Int, int>(direction,
                Util.ManhattanDistance((headPos + direction), target)))
            .OrderBy(tuple => tuple.Item2)
            .First()
            .Item1;
    }
}
}