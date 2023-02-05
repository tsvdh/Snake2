﻿using System;
using System.Collections.Generic;
using TileData;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private Tilemap _tilemap;

    [SerializeField] 
    private List<TileDataHolder> dataHolders;

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

    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = _tilemap.WorldToCell(mousePos);
            
            // FindObjectOfType<AppleManager>().SpawnApple(gridPosition);
            
            // TileBase tile = tilemap.GetTile(gridPosition);
            // print($"Clicked at {gridPosition} on {tile}");
        }
    }

    public TileDataHolder GetTileData(Vector3Int pos)
    {
        return GetTileData(_tilemap.GetTile(pos));
    }

    public TileDataHolder GetTileData(TileBase tileBase)
    {
        return _tileData[tileBase];
    }
}