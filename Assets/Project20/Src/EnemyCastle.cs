

using System;
using System.Collections.Generic;
using System.Linq;
using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public class EnemyCastle : Castle
    {
        public override void Create(CmsEntity cmsEntity, Team team)
        {
            base.Create(cmsEntity, team);

            foreach (var i in cmsEntity.GetAllComponentsOfType<CmsCreateBuildingComp>())
            {
                StartConstructingBuilding(i.building.GetCmsEntity(), i.position);
            }
        }

        public override void Update()
        {
            base.Update();

            if (Vars.player && Vars.player.castle)
            {
                rb.linearVelocity = (Vars.player.castle.transform.position - transform.position).normalized;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }


    [Serializable]
    public class CmsEnemySpawnerComp : CmsComponent
    {
        public float minTime;
        public List<CmsCastleSpawns> castleSpawns = new();
        public float timeBetweenSpawns;

        [Serializable]
        public class CmsCastleSpawns
        {
            public float chance;
            
            [Serializable]
            public class CmsCastleSpawn
            {
                public int count;
                public CmsEntityPfb castle;
            } 
    
            public List<CmsCastleSpawn> castleSpawns = new();
        }
    }

    public class EnemySpawner
    {
        public CmsEntity profile;

        public float progress = 0.0f;        
        public bool active = true;

        public void Init()
        {
            profile = LevelsSystem.level;
        }

        public void Restart()
        {
            active = true;
            progress = 0.0f;
        }

        public void Update()
        {
            if (!Vars.player || !Vars.player.castle)
            {
                return;
            }
            if (!active)
            {
                return;
            }

            CmsEnemySpawnerComp comp = null;
            
            foreach (var i in profile.GetAllComponentsOfType<CmsEnemySpawnerComp>())
            {
                if (Vars.sessionTimer.GetTime() >= i.minTime && (comp == null || comp.minTime < i.minTime))
                {
                    comp = i;
                }
            }

            if (comp != null)
            {
                progress += Time.deltaTime / comp.timeBetweenSpawns;

                if (progress >= 1.0f)
                {
                    float random = UnityEngine.Random.Range(0.0f, 1.0f);

                    CmsEnemySpawnerComp.CmsCastleSpawns castleSpawns = null;

                    float tmp = 0;

                    foreach (var i in comp.castleSpawns)
                    {
                        tmp += i.chance;

                        if (random >= tmp - i.chance && random <= tmp)
                        {
                            castleSpawns = i;
                            break;
                        }
                    }

                    foreach (var i in castleSpawns.castleSpawns)
                    {
                        for (int j = 0; j < i.count; j++)
                        {
                            Vector2 tPos = Vars.player.castle.transform.position;
                            Vector2 pos = Vector2.zero;
                            do
                            {
                                pos = tPos + new Vector2(UnityEngine.Random.Range(-20.0f, 20.0f), UnityEngine.Random.Range(-20.0f, 20.0f));
                            }
                            while (Vector2.Distance(tPos, pos) <= 10.0f);
                            Vars.teams.enemy.castles.StartConstructing(i.castle.GetCmsEntity(), pos);
                            progress = 0.0f;
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public class CmsCreateBuildingComp : CmsComponent
    {
        public Vector2Int position;
        public CmsEntityPfb building;
    }

    [Serializable]
    public class CmsLevelDurationComp : CmsComponent
    {
        public float duration;
    }
}