using System;
using System.Collections.Generic;
using Snake.Strategy;
using TileData;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Snake
{ 
public class SnakeManager : MonoBehaviour
{
    [SerializeField] private List<KeyCode> upButtons;
    [SerializeField] private List<KeyCode> downButtons;
    [SerializeField] private List<KeyCode> leftButtons;
    [SerializeField] private List<KeyCode> rightButtons;

    [SerializeField] private float moveInterval;
    private float _timeSinceLastMove;
    
    private Tilemap _tilemap;
    private MapManager _mapManager;
    private AppleManager _appleManager;
    private SnakeParts _parts;
    private TileBase _bodyTile;
    private Vector3Int? _firstDirection;
    private Vector3Int? _secondDirection;

    [SerializeField] private StrategyType strategyType;
    private MoveStrategy _moveStrategy;

    private void Awake()
    {
        _tilemap = GameObject.Find("Grid/Objects").GetComponent<Tilemap>();
        _mapManager = FindObjectOfType<MapManager>();
        _appleManager = FindObjectOfType<AppleManager>();
        _parts = new SnakeParts();
        _bodyTile = Resources.Load<TileBase>("Tiles/Body");

        BoundsInt bounds = _tilemap.cellBounds;
        _moveStrategy = strategyType switch
        {
            StrategyType.Simple => new SimpleStrategy(bounds),
            StrategyType.AStar => new AStarStrategy(bounds, false, false),
            StrategyType.AStarNoSep => new AStarStrategy(bounds, true, false),
            StrategyType.AStarAllPaths => new AStarStrategy(bounds, false, true),
            StrategyType.AStarNoSepAllPaths => new AStarStrategy(bounds, true, true),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void Start()
    {
        Vector3Int? tailPos = null;

        // Determine tail position
        BoundsInt bounds = _tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y);
                TileBase tile = _tilemap.GetTile(pos);
                if (tile && _mapManager.GetTileData(tile).snake && tile.name.Equals("Tail"))
                {
                    tailPos = pos;
                }
            }
        }

        if (!tailPos.HasValue)
            return;

        Vector3Int curPos = tailPos.Value;

        // for loop to prevent infinite looping
        // follow directions to build snake parts
        for (var i = 0; i < 1000; i++)
        {
            TileBase tile = _tilemap.GetTile(curPos);
            if (!tile || !_mapManager.GetTileData(tile).snake)
                break;

            Vector3Int direction = Util.QuaternionToDirection(_tilemap.GetTransformMatrix(curPos).rotation);
            _parts.AddLast(new SnakePart { Pos = curPos, Direction = direction, Tile = tile });

            curPos += direction;
        }
    }

    private void Update()
    {
        // listen to input
        Vector3Int? directionPressed = GetDirectionPressed();
        if (directionPressed.HasValue)
        {
            if (!_firstDirection.HasValue)
                _firstDirection = directionPressed;

            else if (!_secondDirection.HasValue)
                _secondDirection = directionPressed;
        }

        // check if time for new move
        _timeSinceLastMove += Time.deltaTime;
        if (_timeSinceLastMove >= moveInterval)
            _timeSinceLastMove = 0;
        else
            return;

        // set new direction of head and set cached press
        Vector3Int direction;
        
        if (_firstDirection.HasValue)
        {
            direction = _firstDirection.Value;

            _firstDirection = _secondDirection;
            _secondDirection = null;
        }
        else
        {
            direction = _moveStrategy.GetDirection(_parts, _appleManager.GetAppleLocation());
        }
        
        SnakePart head = _parts.Last.Value;
        head.Direction = direction;
        _parts.Last.Value = head;

        // check new head pos for actions (collision, apple)
        SnakeParts oldParts = _parts.Clone();
        Vector3Int newHeadPos = _parts.Last.Value.Pos + direction;
        TileDataHolder dataHolder = _mapManager.GetTileData(newHeadPos);
        if (dataHolder.collide && newHeadPos != _parts.First.Value.Pos)
        {
            moveInterval = 1000;
            print($"Game over! Score: {_parts.Count - 3}");
            return;
        }
        if (dataHolder.grow)
        {
            // move only head and add part
            _parts.AddBefore(_parts.Last,
                new SnakePart { Direction = head.Direction, Pos = head.Pos, Tile = _bodyTile });

            head.Pos = newHeadPos;
            _parts.Last.Value = head;
        }
        else
            _parts.MoveInPlace();

        // clear old snake tiles
        foreach (SnakePart part in oldParts)
        {
            _tilemap.SetTile(part.Pos, null);
        }
        // repaint snake on new positions
        foreach (SnakePart part in _parts)
        {
            _tilemap.SetTile(new TileChangeData(
                part.Pos,
                part.Tile,
                Color.white,
                Matrix4x4.Rotate(Util.DirectionToQuaternion(part.Direction))
            ), true);
        }
    }

    

    private Vector3Int? GetDirectionPressed()
    {
        bool AnyPressed(List<KeyCode> codes)
        {
            foreach (KeyCode code in codes)
            {
                if (Input.GetKeyDown(code))
                    return true;
            }

            return false;
        }

        if (AnyPressed(upButtons))
            return Vector3Int.up;
        if (AnyPressed(leftButtons))
            return Vector3Int.left;
        if (AnyPressed(downButtons))
            return Vector3Int.down;
        if (AnyPressed(rightButtons))
            return Vector3Int.right;

        return null;
    }
}
}