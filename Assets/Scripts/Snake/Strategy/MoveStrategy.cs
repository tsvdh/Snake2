using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Snake.Strategy
{
public abstract class MoveStrategy
{
    private Tilemap _tilemap;
    private TileBase _emptyTile;
    private TileBase _pathTile;
    
    protected BoundsInt Bounds;
    protected MapManager MapManager;
    
    protected Queue<SnakePart> Path;

    protected MoveStrategy(BoundsInt bounds)
    {
        _tilemap = GameObject.Find("Grid/Indicators").GetComponent<Tilemap>();
        _pathTile = Resources.Load<TileBase>("Tiles/Path");

        Bounds = bounds;
        MapManager = UnityEngine.Object.FindObjectOfType<MapManager>();
        Path = new Queue<SnakePart>();
    }
    
    public abstract Vector3Int GetDirection(SnakeParts parts, Vector3Int target);
    
    protected void PaintPath(int start)
    {
        for (int x = Bounds.xMin; x < Bounds.xMax; x++)
        {
            for (int y = Bounds.yMin; y < Bounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y);
                _tilemap.SetTile(pos, null);
            }
        }

        SnakePart[] parts = Path.ToArray();
        for (int i = start; i < parts.Length; i++)
        {
            _tilemap.SetTile(new TileChangeData(
                parts[i].Pos, 
                _pathTile,
                Color.white,
                Matrix4x4.Rotate(Util.DirectionToQuaternion(parts[i].Direction))
            ), true);

        }
    }
}
}