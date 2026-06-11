using System;
using System.Collections.Generic;
using System.Drawing;
using Blaze.Runtime.Cms;
using Unity.VisualScripting;
using UnityEngine;

namespace Proj21
{   
    public interface IPositionProvider
    {
        public Vector2 GetPosition();
    }

    public class Castle : MonoBehaviour, IPositionProvider
    {
        public CmsEntity cmsEntity;

        [NonSerialized] public Vector2Int size;
        [NonSerialized] public List<Tile> tiles = new();

        public TeamComp teamC;
        public CastleHealthComp healthC;

        public BoxCollider2D _collider;
        public Rigidbody2D rb;

        public SpriteRenderer shadowSpriteRenderer;

        [NonSerialized] public List<Building> buildings = new();

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
                    tile.castle = this;
                    tile.pos = new Vector2Int(x, y);
                }
            }

            Init();
        }

        public virtual void Init()
        {
            _collider.size = new Vector2(size.x, size.y);
            _collider.offset = CenterPosition - (Vector2)transform.position;

            healthC.SetFromCmsEntity(cmsEntity);
            healthC.Set1(this);
            healthC.Init();
            healthC.onDie.AddListener(() => Destroy(gameObject));
        
            shadowSpriteRenderer.transform.position = CenterPosition - new Vector2(0.25f, 0.25f);
            shadowSpriteRenderer.transform.localScale = new Vector2(size.x, size.y);
        }

        public virtual void Update()
        {

        }

        public virtual void OnDestroy()
        {
            teamC.team.castles.castles.Remove(this);
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

        public bool CanPlaceBuilding(Vector2Int pos, int size)
        {
            if (!IsPositionInBounds(pos) || !IsPositionInBounds(pos + new Vector2Int(size, size) - Vector2Int.one))
            {
                return false;
            }
            foreach (var t in GetTilesInRect(new RectInt(pos, new Vector2Int(size, size))))
            {
                if (t.building)
                {
                    return false;
                }
            }
            return true;
        }

        public Building CreateBuilding(CmsEntity build, Vector2Int pos)
        {
            int size = build.GetComponent<CmsSquareSizeComp>().size;
            Vector2 wPos = GetRectPosition(pos, size);
            var b = Instantiate(build.GetComponent<CmsPfbComp>().pfb, wPos, Quaternion.identity, transform).GetComponent<Building>();
            b.castle = this;
            b.pos = pos;
            b.cmsEntity = build;
            foreach (var t in GetTilesInRect(new RectInt(pos, new Vector2Int(size, size))))
            {
                t.building = b;
            }
            b.teamC.Set(teamC.team);
            b.Init();
            buildings.Add(b);
            return b;
        }

        public void DestroyBuilding(Building build)
        {
            buildings.Remove(build);
            foreach (var t in GetTilesInRect(build.GetRect()))
            {
                t.building = null;
            }
            Destroy(build.gameObject);
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

    public class CastlesSystem
    {
        public Team team;
        public List<Castle> castles = new();

        public CastlesSystem(Team team)
        {
            this.team = team;
        }

        public Castle Create(CmsEntity cmsEntity, Vector2 position)
        {
            var inst = GameObject.Instantiate(cmsEntity.GetComponent<CmsPfbComp>().pfb, position, Quaternion.identity).GetComponent<Castle>();
            inst.Create(cmsEntity, team);
            castles.Add(inst);
            Vars.restart.destroyOnRestart.Add(inst.gameObject);
            return inst;
        }

        public void Destroy(Castle castle)
        {
            Vars.effects.CreateEffect(Vars.effects.buildingDestroyEffect, castle.CenterPosition);
            castles.Remove(castle);
            Vars.restart.destroyOnRestart.Remove(castle.gameObject);
            GameObject.Destroy(castle.gameObject);
        }
    }


    [Serializable]
    public class CmsTeamComp : CmsComponent
    {
        public CmsEntityPfb team;
    }

    [Serializable]
    public class CmsSizeIComp : CmsComponent
    {
        public Vector2Int size;
    }

    [Serializable]
    public class CmsCastleTilesComp : CmsComponent
    {
        public CmsEntityPfb tileA;
        public CmsEntityPfb tileB;
    }

    [Serializable]
    public class CmsSquareSizeComp : CmsComponent
    {
        public int size;
    }

    [Serializable]
    public class CmsSpriteComp : CmsComponent
    {
        public Sprite sprite;
    }
}