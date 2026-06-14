

using System;
using UnityEngine;

namespace Proj21
{
    public class Tile : MonoBehaviour
    {
        [NonSerialized] public CastleGrid grid;
        [NonSerialized] public Vector2Int pos;
        [NonSerialized] public Building building = null;
        public SpriteRenderer spriteRenderer;
    }
}