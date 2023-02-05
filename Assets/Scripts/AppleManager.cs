
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AppleManager : MonoBehaviour
{
    private TileBase _appleTile;

    private void Awake()
    {
        _appleTile = Resources.Load<TileBase>("Tiles/Apple");
    }

    public void SpawnApple(Vector3Int pos)
    {
        var mapManager = FindObjectOfType<MapManager>();
        mapManager.SetTile(pos, _appleTile);
    }
}