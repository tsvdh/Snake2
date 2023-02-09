using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Snake
{
public struct SnakePart
{
    public Vector3Int Pos;
    public Vector3Int Direction;
    public TileBase Tile;
    
    public SnakePart Clone()
    {
        return new SnakePart { Direction = Direction, Pos = Pos, Tile = Tile };
    }
}
}