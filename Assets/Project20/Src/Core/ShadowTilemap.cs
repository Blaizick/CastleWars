using UnityEngine;
using UnityEngine.Tilemaps;

namespace Blaze
{
    public class ShadowTilemap : MonoBehaviour
    {
        public Tilemap tilemap;
        public Tilemap shadowTilemap;

        public void Init()
        {
            BoundsInt bounds = tilemap.cellBounds;

            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(pos);

                if (tile != null)
                {
                    shadowTilemap.SetTile(pos, tile);
                }
            }    
        }
    }
}