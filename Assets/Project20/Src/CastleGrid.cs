using System;
using System.Collections.Generic;
using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public class CastleGrid : MonoBehaviour, IPositionProvider
    {
        public CmsEntity cmsEntity;

        [NonSerialized] public Vector2Int size;
        [NonSerialized] public List<Tile> tiles = new();

        public TeamComp teamC;

        public SpriteRenderer shadowSpriteRenderer;
        public SpriteRenderer outlineSpriteRenderer;

        public virtual void Create(CmsEntity cmsEntity, Team team)
        {
            this.cmsEntity = cmsEntity;
            teamC.Set(team);

            size = cmsEntity.GetComponent<CmsSizeIComp>().size;

            tiles = new(new Tile[size.x * size.y]);
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var castleTileA = cmsEntity.GetComponent<CmsCastleTilesComp>().tileA.GetCmsEntity();
                    var castleTileB = cmsEntity.GetComponent<CmsCastleTilesComp>().tileB.GetCmsEntity();

                    var pfb = (x + (y * size.x) + (size.x % 2 == 0 ? y : 0)) % 2 == 0 ? castleTileA.GetComponent<CmsPfbComp>().pfb : castleTileB.GetComponent<CmsPfbComp>().pfb;
                    var tile = Instantiate(pfb, GridToWorldPosition(new Vector2Int(x, y)), Quaternion.identity, transform).GetComponent<Tile>();
                    tiles[x + y * size.x] = tile;
                    tile.grid = this;
                    tile.pos = new Vector2Int(x, y);
                }
            }

            Init();
        }

        public virtual void Init()
        {
            outlineSpriteRenderer.transform.position = CenterPosition;
            outlineSpriteRenderer.transform.localScale = new Vector2(size.x + 0.2f, size.y + 0.2f);

            shadowSpriteRenderer.transform.position = CenterPosition - new Vector2(0.2f, 0.2f);
            shadowSpriteRenderer.transform.localScale = new Vector2(size.x + 0.2f, size.y + 0.2f);
        }

        public virtual void Update()
        {

        }

        public virtual void OnDestroy()
        {
            
        }

        public Vector2 GridToWorldPosition(Vector2Int gPos)
        {
            return (Vector2)gPos + (Vector2)transform.position;
        }

        public Vector2Int WorldToGridPosition(Vector2 pos)
        {
            Vector2 tmp = pos - (Vector2)transform.position;
            if (tmp.x > 0.0f)
            {
                tmp.x += 0.5f;
            }
            if (tmp.y > 0.0f)
            {
                tmp.y += 0.5f;
            }
            if (tmp.x < 0.0f)
            {
                tmp.x -= 0.5f;                
            }
            if (tmp.y < 0.0f)
            {
                tmp.y -= 0.5f;
            }
            return new Vector2Int((int)tmp.x, (int)tmp.y);
        }

        public Vector2 GetRectPosition(Vector2Int pos, int size)
        {
            int hs = size / 2;
            return GridToWorldPosition(pos + new Vector2Int(hs, hs)) - (size % 2 == 0 ? Vector2.one / 2 : Vector2.zero);
        }

        public List<Tile> GetTilesInRect(RectInt rect)
        {
            List<Tile> list = new();
            for (int x = 0; x < rect.width; x++)
            {
                for (int y = 0; y < rect.height; y++)
                {
                    int cx = x + rect.x;
                    int cy = y + rect.y;
                    if (IsPositionInBounds(new Vector2Int(cx, cy)))
                    {
                        list.Add(tiles[GridPosToI(new Vector2Int(cx, cy))]);
                    }
                }
            }
            return list;
        }

        public bool IsPositionInBounds(Vector2Int pos)
        {
            if (pos.x < 0 || pos.y < 0 ||
                pos.x >= this.size.x || pos.y >= this.size.y)
            {
                return false;
            }
            return true;
        }

        public int GridPosToI(Vector2Int gPos)
        {
            return gPos.x + gPos.y * size.x;
        }

        public Vector2 GetPosition()
        {
            return CenterPosition;
        }

        public Vector2 CenterPosition
        {
            get
            {
                Vector2 pos = (Vector2)transform.position + new Vector2(size.x / 2, size.y / 2);
                if (size.x % 2 == 0)
                {
                    pos.x -= 0.5f;
                }
                if (size.y % 2 == 0)
                {
                    pos.y -= 0.5f;
                }
                return pos;
            }
        }
    }
}