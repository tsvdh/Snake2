using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Snake.Strategy
{
public class EdgeStrategy : MoveStrategy
{
    private enum Mode
    {
        ToEdge,
        OnEdge,
        ToTarget
    }
    
    // private readonly float _turn = 90;
    
    public EdgeStrategy(BoundsInt bounds) : base(bounds) { }

    public override Vector3Int GetDirection(SnakeParts parts, Vector3Int target)
    {
        // if path exists return first step
        if (Path.Count > 0)
        {
            PaintPath(2);
            return Path.Dequeue().Direction;
        }
        
        long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        
        // always go to one exit tiles
        // if going to adjacent tile creates separation, go in direction of smaller separation 
        
        // to edge:
        //      determine edge tiles
        //      if on edge, switch mode
        //      else go straight unless impossible, go right
        
        //
        
        return Vector3Int.zero;
    }
}
}