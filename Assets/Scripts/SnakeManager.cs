using System;
using System.Collections.Generic;
using TileData;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SnakeManager : MonoBehaviour
{
    private struct SnakePart
    {
        public Vector3Int Pos;
        public Vector3Int Direction;
        public TileBase Tile;
    }
    
    [SerializeField] private float moveInterval;
    private float _timeSinceLastMove;
    
    private Tilemap _tilemap; 
    private MapManager _mapManager;
    private LinkedList<SnakePart> _parts;
    private TileBase _bodyTile;

    private void Awake()
    {
        _tilemap = GameObject.Find("Grid/Objects").GetComponent<Tilemap>();
        _mapManager = FindObjectOfType<MapManager>();
        _parts = new LinkedList<SnakePart>();
        _bodyTile = Resources.Load<TileBase>("Tiles/Body");
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
            
            var direction = (Vector3Int)QuaternionToDirection(_tilemap.GetTransformMatrix(curPos).rotation);
            _parts.AddLast(new SnakePart{Pos = curPos, Direction = direction, Tile = tile});
            
            curPos += direction;
        }
    }

    private void Update()
    {
        // listen to input
        SnakePart head = _parts.Last.Value;
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            head.Direction = Vector3Int.up;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            head.Direction = Vector3Int.left;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            head.Direction = Vector3Int.down;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            head.Direction = Vector3Int.right;
        
        _parts.Last.Value = head;
        
        // check if time for new move
        _timeSinceLastMove += Time.deltaTime;
        if (_timeSinceLastMove < moveInterval)
            return;
        
        _timeSinceLastMove = 0;

        // check new head pos for actions (collision, apple)
        Vector3Int newHeadPos = _parts.Last.Value.Pos + _parts.Last.Value.Direction;
        TileDataHolder dataHolder = _mapManager.GetTileData(newHeadPos);
        if (dataHolder.collide)
        {
            moveInterval = 1000;
            print("Game over!");
            return;
        }

        // clear snake tiles
        foreach (SnakePart part in _parts)
        {
            _tilemap.SetTile(part.Pos, null);
        }
        
        if (dataHolder.grow)
        {
            // move only head and add part
            _parts.AddBefore(_parts.Last, new SnakePart{Direction = head.Direction, Pos = head.Pos, Tile = _bodyTile});
            
            head.Pos = newHeadPos;
            _parts.Last.Value = head;
        }
        else
        {
            // update all snake parts position and rotation
            LinkedListNode<SnakePart> curNode = _parts.First;
            while (curNode != null)
            {
                SnakePart curPart = curNode.Value;

                curPart.Pos += curPart.Direction;

                if (curNode.Next != null)
                    curPart.Direction = curNode.Next.Value.Direction;

                curNode.Value = curPart;
                curNode = curNode.Next;
            }
        }

        // repaint snake on new positions
        foreach (SnakePart part in _parts)
        {
            _tilemap.SetTile(new TileChangeData(
                part.Pos,
                part.Tile,
                Color.white,
                Matrix4x4.Rotate(DirectionToQuaternion((Vector2Int)part.Direction))
            ), true);
        }
    }

    private static Vector2Int QuaternionToDirection(Quaternion quaternion)
    {
        // Default direction is to the right
        return quaternion.eulerAngles.z switch
        {
            0 => Vector2Int.right,
            90 => Vector2Int.up,
            180 => Vector2Int.left,
            270 => Vector2Int.down,
            _ => throw new ArgumentException("must be one of the four primary angles", nameof(quaternion))
        };
    }

    private static Quaternion DirectionToQuaternion(Vector2Int direction)
    {
        return direction switch
        {
            var v when v.Equals(Vector2Int.right) => Quaternion.Euler(0, 0, 0),
            var v when v.Equals(Vector2Int.up) => Quaternion.Euler(0, 0, 90),
            var v when v.Equals(Vector2Int.left) => Quaternion.Euler(0, 0, 180),
            var v when v.Equals(Vector2Int.down) => Quaternion.Euler(0, 0, 270),
            _ => throw new ArgumentException("must be one of the four primary directions", nameof(direction))
        };
    }
}