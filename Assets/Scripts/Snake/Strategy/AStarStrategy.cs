using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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

    private BoundsInt _bounds;
    private Queue<SnakePart> _path;

    private bool _noSeparateSpaces;
    private bool _morePaths;

    public AStarStrategy(BoundsInt bounds, bool noSeparateSpaces, bool morePaths)
    {
        _tilemap = GameObject.Find("Grid/Indicators").GetComponent<Tilemap>();
        _pathTile = Resources.Load<TileBase>("Tiles/Path");
        _bounds = bounds;
        _path = new Queue<SnakePart>();

        _noSeparateSpaces = noSeparateSpaces;
        _morePaths = morePaths;
    }

    public override Vector3Int GetDirection(SnakeParts parts, BoundsInt bounds, Vector3Int apple)
    {
        // if path exists return first step
        if (_path.Count > 0)
        {
            PaintPath(2);
            return _path.Dequeue().Direction;
        }
        
        long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // else compute new path
        var options = new PriorityQueue<Tuple<SnakeParts, SnakeParts>, int>();
        int distance = ManhattanDistance(parts.Last.Value.Pos, apple);
        options.Enqueue(new Tuple<SnakeParts, SnakeParts>(parts, new SnakeParts()), distance);
        
        var visited = new HashSet<Vector3Int>();

        while (options.Count > 0)
        {
            (SnakeParts curParts, SnakeParts curPath) = options.Dequeue();
            Vector3Int curHead = curParts.Last.Value.Pos;
            visited.Add(curHead);
            
            if (curHead.Equals(apple))
            {
                long duration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - start;
                Debug.Log($"Decision took: {duration}");
                
                curPath.ToList().ForEach(_path.Enqueue);
                PaintPath(2);
                return _path.Dequeue().Direction;
            }

            void AddOption(Vector3Int possibleDir)
            {
                SnakeParts newParts = curParts.CloneAndMove(possibleDir);
                SnakeParts newPath = curPath.Clone();
                newPath.AddLast(new SnakePart { Pos = curHead, Direction = possibleDir });
                int newDistance = newPath.Count + ManhattanDistance(curHead + possibleDir, apple);
                
                options.Enqueue(new Tuple<SnakeParts, SnakeParts>(newParts, newPath), newDistance);
            }
            
            LinkedList<Vector3Int> possibleDirs = GetPossibleDirections(curParts, bounds, curHead);

            var addMore = true;
            if (_noSeparateSpaces)
            {
                foreach (Vector3Int possibleDir in possibleDirs)
                {
                    LinkedList<Vector3Int> adjacentPossibleDirs =
                        GetPossibleDirections(curParts, bounds, curHead + possibleDir);

                    if (adjacentPossibleDirs.Count != 1) 
                        continue;
                    
                    AddOption(possibleDir);
                    addMore = false;
                    break;
                }
            }

            if (!addMore) 
                continue;
            
            foreach (Vector3Int possibleDir in possibleDirs)
            {
                if (visited.Contains(curHead + possibleDir))
                    continue;
                
                AddOption(possibleDir);
            }
        }

        Debug.Log("Could not find path");
        return Vector3Int.zero;
    }

    private void PaintPath(int start)
    {
        for (int x = _bounds.xMin; x < _bounds.xMax; x++)
        {
            for (int y = _bounds.yMin; y < _bounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y);
                _tilemap.SetTile(pos, null);
            }
        }

        SnakePart[] parts = _path.ToArray();
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