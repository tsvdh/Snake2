using System;
using System.Collections.Generic;
using TileData;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] private List<TileDataHolder> dataHolders;
    
    private Tilemap _tilemap;
    private Dictionary<TileBase, TileDataHolder> _tileData;

    private void Awake()
    {
        _tilemap = GameObject.Find("Grid/Objects").GetComponent<Tilemap>();

        _tileData = new Dictionary<TileBase, TileDataHolder>();
        foreach (TileDataHolder dataHolder in dataHolders)
        {
            foreach (TileBase tile in dataHolder.tiles)
            {
                _tileData.Add(tile, dataHolder);
            }
        }
    }

    public TileDataHolder GetTileData(Vector3Int pos)
    {
        return GetTileData(_tilemap.GetTile(pos));
    }

    public TileDataHolder GetTileData(TileBase tileBase)
    {
        return tileBase
            ? _tileData[tileBase] 
            : ScriptableObject.CreateInstance<TileDataHolder>();
    }
}