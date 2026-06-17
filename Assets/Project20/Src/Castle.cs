using System;
using System.Collections;
using System.Collections.Generic;
using Blaze.Runtime.Cms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Proj21
{   
    public interface IPositionProvider
    {
        public Vector2 GetPosition();
    }

    public class Castle : CastleGrid
    {
        public CastleHealthComp healthC;

        public BoxCollider2D _collider;
        public Rigidbody2D rb;

        [NonSerialized] public List<Building> buildings = new();

        public override void Init()
        {
            base.Init();

            _collider.size = new Vector2(size.x, size.y);
            _collider.offset = CenterPosition - (Vector2)transform.position;

            if (healthC)
            {
                healthC.SetFromCmsEntity(cmsEntity);
                healthC.Set1(this);
                healthC.Init();
                healthC.onDie.AddListener(() => teamC.team.castles.StartDeconstructing(this));
            }
        }

        public override void Update()
        {
            base.Update();
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

        public Building CreateBuilding(CmsEntity build, Vector2Int pos, int size, GameObject prefab)
        {
            Vector2 wPos = GetRectPosition(pos, size);
            var b = Instantiate(prefab, wPos, Quaternion.identity, transform).GetComponent<Building>();
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
        public Building CreateBuilding(CmsEntity build, Vector2Int pos)
        {
            return CreateBuilding(build, 
                pos, 
                build.GetComponent<CmsSquareSizeComp>().size, 
                build.GetComponent<CmsPfbComp>().pfb
                );
        }

        public void FinishCostructingBuilding(ConstructBuilding constructBuilding)
        {
            DestroyBuilding(constructBuilding);
            var cmsEnt = constructBuilding.cmsEntity;
            var pos = constructBuilding.pos;
            CreateBuilding(cmsEnt, pos);
        }

        public void StartConstructingBuilding(CmsEntity build, Vector2Int pos)
        {
            var constructBuildEnt = Cms.GetEntity("ConstructBuilding");
            int size = build.GetComponent<CmsSquareSizeComp>().size;
            var pfb = constructBuildEnt.GetComponent<CmsPfbComp>().pfb;
            CreateBuilding(build, pos, size, pfb);
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
    }

    public class CastlesSystem
    {
        public Team team;
        public List<Castle> castles = new();

        public CastlesSystem(Team team)
        {
            this.team = team;
        }

        public AppearCastle StartConstructing(CmsEntity cmsEntity, Vector2 position)
        {
            var inst = (AppearCastle)CreateUninitialized(cmsEntity, position, Resources.Load<GameObject>("Pfb/Castles/AppearCastle"));
            // var castle = (AppearCastle)Create(cmsEntity, position, cmsEntity.GetComponent<CmsAppearCastlePfbComp>().pfb);
            inst._operator = new ConstructCastleOperator();
            inst.Create(cmsEntity, team);
            return inst;
        }

        public IEnumerator StartConstructingCoroutine(CmsEntity cmsEntity, Vector2 position, UnityAction<Castle> callback)
        {
            var castle = StartConstructing(cmsEntity, position);
            // Debug.Log(castle._operator.GetType());
            // Debug.Log(((ConstructCastleOperator)castle._operator).onAppear.GetType());
            // ((ConstructCastleOperator)castle._operator).onAppear.AddListener(d => Debug.Log(d.name));
            ((ConstructCastleOperator)castle._operator).onAppear.AddListener(_castle => callback?.Invoke(_castle));
            while (castle)
            {
                yield return null;
            }
        }

        public Castle Create(CmsEntity cmsEntity, Vector2 position, GameObject prefab)
        {
            var inst = CreateUninitialized(cmsEntity, position, prefab);
            inst.Create(cmsEntity, team);
            return inst;
        }

        public Castle Create(CmsEntity cmsEntity, Vector2 position)
        {
            return Create(cmsEntity, position, cmsEntity.GetComponent<CmsPfbComp>().pfb);
        }

        public Castle CreateUninitialized(CmsEntity cmsEntity, Vector2 position, GameObject prefab)
        {
            var inst = GameObject.Instantiate(prefab, position, Quaternion.identity).GetComponent<Castle>();
            castles.Add(inst);
            Vars.restart.destroyOnRestart.Add(inst.gameObject);
            return inst;
        }

        public void StartDeconstructing(Castle castle)
        {
            var inst = (AppearCastle)CreateUninitialized(castle.cmsEntity, castle.transform.position, Resources.Load<GameObject>("Pfb/Castles/AppearCastle"));
            inst._operator = new DeconstructCastleOperator();
            inst.Create(castle.cmsEntity, team);
            Destroy(castle);
            // Vars.effects.CreateEffect(Vars.effects.buildingDestroyEffect, castle.CenterPosition);
            // DestroyQuiet(castle);
        }

        public void Destroy(Castle castle)
        {
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

    [Serializable]
    public class CmsAppearCastlePfbComp : CmsComponent
    {
        public GameObject pfb;
    }
}