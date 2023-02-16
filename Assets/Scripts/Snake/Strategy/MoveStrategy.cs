using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snake.Strategy
{
public interface MoveStrategy
{
    public Vector3Int GetDirection(SnakeParts parts, Vector3Int target);
}
}