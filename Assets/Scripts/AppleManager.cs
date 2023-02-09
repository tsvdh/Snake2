using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class AppleManager : MonoBehaviour
{
    private Tilemap _tilemap;
    private TileBase _appleTile;
    private Vector3Int _appleLocation;

    private void Awake()
    {
        _tilemap = GameObject.Find("Grid/Objects").GetComponent<Tilemap>();
        _appleTile = Resources.Load<TileBase>("Tiles/Apple");
    }

    private void Start()
    {
        Update();
    }

    private void Update()
    {
        if (!AppleExists())
            SpawnApple();
    }
    
    public Vector3Int GetAppleLocation()
    {
        return _appleLocation;
    }

    public bool AppleExists()
    {
        BoundsInt bounds = _tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                TileBase tile = _tilemap.GetTile(new Vector3Int(x, y));
                if (tile && tile.name.Equals("Apple"))
                {
                    _appleLocation = new Vector3Int(x, y);
                    return true;
                }
            }
        }

        return false;
    }

    private void SpawnApple()
    {
        BoundsInt bounds = _tilemap.cellBounds;
        
        // for loop to prevent infinite looping
        for (var i = 0; i < 10000; i++)
        {
            var location = new Vector3Int(Random.Range(bounds.xMin, bounds.xMax), 
                                          Random.Range(bounds.yMin, bounds.yMax));
            TileBase tile = _tilemap.GetTile(location);

            if (!tile)
            {
                _tilemap.SetTile(location, _appleTile);
                break;
            }
        }
    }
}