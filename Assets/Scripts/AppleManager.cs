
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class AppleManager : MonoBehaviour
{
    private Tilemap _tilemap;
    private TileBase _appleTile;
    private MapManager _mapManager;

    private void Awake()
    {
        _tilemap = GameObject.Find("Grid/Objects").GetComponent<Tilemap>();
        _appleTile = Resources.Load<TileBase>("Tiles/Apple");
        _mapManager = FindObjectOfType<MapManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnApple();
        }
    }

    public void SpawnApple()
    {
        while (true)
        {
            var location = new Vector3Int(Random.Range(-7, 7), Random.Range(-7, 7));
            TileBase tile = _tilemap.GetTile(location);

            if (!tile)
            {
                _tilemap.SetTile(location, _appleTile);
                break;
            }
        }
    }
}