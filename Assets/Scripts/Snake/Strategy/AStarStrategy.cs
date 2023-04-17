using System;
using System.Collections.Generic;
using System.Linq;
using TileData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Snake.Strategy
{

internal class VisitedManager
{
    private bool _allPaths;
    private HashSet<Vector3Int> _positions;
    private HashSet<SnakeParts> _paths;

    internal VisitedManager(bool allPaths)
    {
        _allPaths = allPaths;
        if (!allPaths)
            _positions = new HashSet<Vector3Int>();
        else
            _paths = new HashSet<SnakeParts>();
    }

    internal void Add(SnakeParts path)
    {
        if (!_allPaths)
        {
            if (path.Last.Value.Direction == Vector3Int.zero)
                _positions.Add(path.Last.Value.Pos);
            else
                _positions.Add(path.Last.Value.Pos + path.Last.Value.Direction);
        }
        
        else 
            _paths.Add(path);
    }

    internal bool Contains(SnakeParts path)
    {
        if (!_allPaths)
        {
            if (path.Last.Value.Direction == Vector3Int.zero)
                return _positions.Contains(path.Last.Value.Pos);
            else
                return _positions.Contains(path.Last.Value.Pos + path.Last.Value.Direction);
        }
        else
            return _paths.Contains(path);
    }
}

public class AStarStrategy : MoveStrategy
{
    private Tilemap _tilemap;
    private TileBase _emptyTile;
    private TileBase _pathTile;

    private MapManager _mapManager;

    private BoundsInt _bounds;
    private Queue<SnakePart> _path;

    private bool _noSeparateSpaces;
    private bool _allPaths;

    public AStarStrategy(BoundsInt bounds, bool noSeparateSpaces, bool allPaths)
    {
        _tilemap = GameObject.Find("Grid/Indicators").GetComponent<Tilemap>();
        _pathTile = Resources.Load<TileBase>("Tiles/Path");
        _mapManager = UnityEngine.Object.FindObjectOfType<MapManager>();
        
        _bounds = bounds;
        _path = new Queue<SnakePart>();

        _noSeparateSpaces = noSeparateSpaces;
        _allPaths = allPaths;
    }

    public Vector3Int GetDirection(SnakeParts parts, Vector3Int target)
    {
        // if path exists return first step
        if (_path.Count > 0)
        {
            PaintPath(2);
            return _path.Dequeue().Direction;
        }
        
        long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // else compute new path
        
        // options: queue of (current snake, path traveled by head), dist to target
        var options = new PriorityQueue<Tuple<SnakeParts, SnakeParts>, int>();
        
        // Init base path with only starting point
        var path = new SnakeParts();
        path.AddLast(new SnakePart { Pos = parts.Last.Value.Pos });
        
        int distance = Util.ManhattanDistance(parts.Last.Value.Pos, target);
        options.Enqueue(new Tuple<SnakeParts, SnakeParts>(parts, path), distance);

        var visited = new VisitedManager(_allPaths);

        var numOptions = 0;

        while (options.Count > 0)
        {
            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - start > 30000)
            {
                Debug.Log("Timed out");
                break;
            }

            numOptions++;
            
            (SnakeParts curParts, SnakeParts curPath) = options.Dequeue();
            Vector3Int curHead = curParts.Last.Value.Pos;
            visited.Add(curPath);
            
            if (curHead.Equals(target))
            {
                long duration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - start;
                Debug.Log($"Decision took: {duration}, {numOptions} considered");
                
                curPath.ToList().ForEach(_path.Enqueue);
                PaintPath(0);
                return _path.Dequeue().Direction;
            }

            void AddOption(Vector3Int possibleDir)
            {
                SnakeParts newParts = curParts.CloneAndMove(possibleDir);
                SnakeParts newPath = curPath.CloneAndGrow(possibleDir);
                int newDistance = newPath.Count + Util.ManhattanDistance(curHead + possibleDir, target);

                options.Enqueue(new Tuple<SnakeParts, SnakeParts>(newParts, newPath), newDistance);
            }
            
            LinkedList<Vector3Int> possibleDirs = Util.GetPossibleDirections(curParts, _bounds, curHead);

            var chosenDirs = new LinkedList<Vector3Int>();
            
            // if possibleDirs is length 3 and possibleDir is not ahead, can only eliminate possibleDir
            // else can force direction

            foreach (Vector3Int possibleDir in possibleDirs)
            {
                if (visited.Contains(curPath.CloneAndGrow(possibleDir)))
                    // already checked path
                    continue;
                
                if (!_noSeparateSpaces)
                {
                    // nothing to be done, add direction
                    chosenDirs.AddLast(possibleDir);
                    continue;
                }
                
                // check for separate space
                
                SnakeParts possibleParts = curParts.CloneAndMove(possibleDir);
                Vector3Int possibleHead = curHead + possibleDir;

                Dictionary<Vector3Int, bool> grid = Util.GetGrid(possibleParts, _bounds);

                var adjacentPossiblePositions = new LinkedList<Vector3Int>();
                foreach (Vector3Int adjacentPossibleDir in Util.GetPossibleDirections(grid, possibleHead))
                {
                    adjacentPossiblePositions.AddLast(possibleHead + adjacentPossibleDir);
                }

                if (adjacentPossiblePositions.Count == 1)
                {
                    // only one way out, must go there
                    chosenDirs.Clear();
                    chosenDirs.AddLast(possibleDir);
                    break;
                }

                Vector3Int a = adjacentPossiblePositions.First.Value;
                adjacentPossiblePositions.RemoveFirst();

                var anySeparated = false;
                foreach (Vector3Int b in adjacentPossiblePositions)
                {
                    if (CheckSeparated(a, b, possibleParts))
                        anySeparated = true;
                }

                if (!anySeparated)
                    chosenDirs.AddLast(possibleDir);


                // if (adjacentPossiblePositions.Count == 2)
                // {
                //     bool sameAxis = adjacentPossiblePositions.First.Value.x == adjacentPossiblePositions.Last.Value.x
                //                     || adjacentPossiblePositions.First.Value.y == adjacentPossiblePositions.Last.Value.y;
                //     TileDataHolder tileData = _mapManager.GetTileData(possibleHead + possibleDir);
                //
                //     if (sameAxis && !tileData.wall)
                //         continue;
                //
                //     if (sameAxis && tileData.wall)
                //     {
                //         Vector3Int a = adjacentPossiblePositions.First.Value;
                //         Vector3Int b = adjacentPossiblePositions.Last.Value;
                //         
                //
                //         if (!connected)
                //         {
                //             chosenDirs.Clear();
                //
                //             if (!searchA.CanVisitNext())
                //                 chosenDirs.AddLast(b - possibleHead);
                //
                //             if (!searchB.CanVisitNext())
                //                 chosenDirs.AddLast(a - possibleHead);
                //
                //             break;
                //         }
                //     }
                //
                //     if (!sameAxis)
                //     {
                //         Vector3Int corner = Util.GetFourthSquare(possibleHead, 
                //             adjacentPossiblePositions.First.Value,
                //             adjacentPossiblePositions.Last.Value);
                //
                //         if (grid[corner])
                //         {
                //             Vector3Int ahead = possibleHead + possibleParts.Last.Value.Direction;
                //             adjacentPossiblePositions.Remove(ahead);
                //             
                //             chosenDirs.Clear();
                //             chosenDirs.AddLast(adjacentPossiblePositions.First.Value - possibleHead);
                //             break;
                //         }
                //     }
                // }
                //
                // if (adjacentPossiblePositions.Count == 3)
                // {
                //     Vector3Int ahead = possibleHead + possibleParts.Last.Value.Direction;
                //     adjacentPossiblePositions.Remove(ahead);
                //
                //     Vector3Int a = adjacentPossiblePositions.First.Value;
                //     Vector3Int b = adjacentPossiblePositions.Last.Value;
                //     
                //     Vector3Int cornerA = Util.GetFourthSquare(possibleHead, ahead, a);
                //     Vector3Int cornerB = Util.GetFourthSquare(possibleHead, ahead, b);
                //
                //     if (grid[cornerA] || grid[cornerB])
                //     {
                //         chosenDirs.Clear();
                //         if (grid[cornerA])
                //         {
                //             chosenDirs.AddLast(a - possibleHead);
                //             break;
                //         }
                //         if (grid[cornerB])
                //         {
                //             chosenDirs.AddLast(b - possibleHead);
                //             break;
                //         }
                //     }
                // }
            }
            
            chosenDirs.ToList().ForEach(AddOption);
        }

        Debug.Log("Could not find path");
        return Vector3Int.zero;
    }

    private bool CheckSeparated(Vector3Int a, Vector3Int b, SnakeParts parts)
    {
        var searchA = new AStarSearch(_bounds, parts, b, a);
        var searchB = new AStarSearch(_bounds, parts, a, b);
        
        var connected = false;
        
        while (!connected)
        {
            searchA.VisitNext();
            searchB.VisitNext();
        
            if (searchA.Found() || searchB.Found())
                connected = true;
        
            if (!searchA.CanVisitNext() || !searchB.CanVisitNext())
                break;
        }

        return !connected;
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