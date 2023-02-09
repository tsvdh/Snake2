using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Snake.Strategy
{
public class AStarStrategy : MoveStrategy
{
    private Tilemap _tilemap;
    private TileBase _emptyTile;
    private TileBase _pathTile;

    public AStarStrategy()
    {
        _tilemap = GameObject.Find("Grid/Base").GetComponent<Tilemap>();
        _emptyTile = Resources.Load<TileBase>("Tiles/Empty");
        _pathTile = Resources.Load<TileBase>("Tiles/Path");
    }

    public override Vector3Int GetDirection(SnakeParts parts, BoundsInt bounds, Vector3Int apple)
    {
        var options = new PriorityQueue<Tuple<SnakeParts, SnakeParts>, int>();
        int distance = ManhattanDistance(parts.Last.Value.Pos, apple);
        options.Enqueue(new Tuple<SnakeParts, SnakeParts>(parts, new SnakeParts()), distance);
        
        var visited = new HashSet<Vector3Int> { parts.Last.Value.Pos };

        while (options.Count > 0)
        {
            (SnakeParts curParts, SnakeParts curPath) = options.Dequeue();
            Vector3Int curHead = curParts.Last.Value.Pos;

            if (curHead.Equals(apple))
            {
                PaintPath(bounds, curPath);
                return curPath.First.Value.Direction;
            }

            visited.Add(curHead);
            
            Dictionary<Vector3Int, bool> grid = GetGrid(curParts, bounds);
            LinkedList<Vector3Int> possibleDirs = GetPossibleDirections(grid, curHead);

            possibleDirs = new LinkedList<Vector3Int>(possibleDirs.Where(
                possibleDir => !visited.Contains(curHead + possibleDir)));

            foreach (Vector3Int possibleDir in possibleDirs)
            {
                SnakeParts newParts = curParts.CloneAndMove(possibleDir);
                SnakeParts newPath = curPath.Clone();
                newPath.AddLast(new SnakePart { Pos = curHead, Direction = possibleDir });
                int newDistance = newPath.Count + ManhattanDistance(curHead + possibleDir, apple);
                
                options.Enqueue(new Tuple<SnakeParts, SnakeParts>(newParts, newPath), newDistance);
            }
        }

        Debug.Log("Could not find path");
        return Vector3Int.zero;
    }

    private void PaintPath(BoundsInt bounds, SnakeParts path)
    {
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y);
                _tilemap.SetTile(pos, _emptyTile);
            }
        }
        foreach (SnakePart part in path)
        {
            _tilemap.SetTile(part.Pos, _pathTile);
        }
    }
}
}