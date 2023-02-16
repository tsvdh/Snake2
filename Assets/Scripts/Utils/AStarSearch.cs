using System.Collections.Generic;
using Snake;
using UnityEngine;

namespace Utils
{
public class AStarSearch
{
    private BoundsInt _bounds;
    private SnakeParts _parts;
    private PriorityQueue<Vector3Int, int> _options;
    private HashSet<Vector3Int> _visited;
    private Vector3Int _target;

    public AStarSearch(BoundsInt bounds, SnakeParts parts, Vector3Int start, Vector3Int target)
    {
        _bounds = bounds;
        _parts = parts;

        _options = new PriorityQueue<Vector3Int, int>();
        _options.Enqueue(start, Util.ManhattanDistance(start, target));
        
        _visited = new HashSet<Vector3Int> { start };
        _target = target;
    }
    
    public void VisitNext()
    {
        Vector3Int position = _options.Dequeue();

        foreach (Vector3Int possibleDir in Util.GetPossibleDirections(_parts, _bounds, position))
        {
            Vector3Int possiblePos = position + possibleDir;
            if (!_visited.Contains(possiblePos))
            {
                _visited.Add(possiblePos);
                _options.Enqueue(possiblePos, Util.ManhattanDistance(possiblePos, _target));
            }
        }
    }

    public bool CanVisitNext()
    {
        return _options.Count > 0;
    }

    public bool Found()
    {
        return _visited.Contains(_target);
    }
}
}