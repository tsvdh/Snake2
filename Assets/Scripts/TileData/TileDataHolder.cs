using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileData
{
    [CreateAssetMenu]
    public class TileDataHolder : ScriptableObject
    {
        public TileBase[] tiles;

        public bool grow, collide, snake, wall;
    }
}