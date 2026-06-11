

using System;
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
                CreateBuilding(i.building.GetCmsEntity(), i.position);
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

    public class EnemySpawner
    {
        public float progress = 0.0f;        

        public void Init()
        {
            
        }

        public void Restart()
        {
            playing = true;
            progress = 0.0f;
        }

        public bool playing = true;

        public void Update()
        {
            if (!Vars.player || !Vars.player.castle)
            {
                return;
            }
            if (!playing)
            {
                return;
            }

            if (Vars.sessionTimer.GetTime() >= 20.0f)
            {
                progress += Time.deltaTime / 5.0f;
                if (progress >= 1.0f)
                {
                    Vector2 tPos = Vars.player.castle.transform.position;
                    Vector2 pos = Vector2.zero;
                    do
                    {
                        pos = tPos + new Vector2(UnityEngine.Random.Range(-20.0f, 20.0f), UnityEngine.Random.Range(-20.0f, 20.0f));
                    }
                    while (Vector2.Distance(tPos, pos) <= 10.0f);
                    Vars.teams.enemy.castles.Create(Cms.GetEntity("EnemyCastle1"), pos);
                    progress = 0.0f;
                }
            }
            else
            {
                progress += Time.deltaTime / 5.0f;
                if (progress >= 1.0f)
                {
                    Vector2 tPos = Vars.player.castle.transform.position;
                    Vector2 pos = Vector2.zero;
                    do
                    {
                        pos = tPos + new Vector2(UnityEngine.Random.Range(-20.0f, 20.0f), UnityEngine.Random.Range(-20.0f, 20.0f));
                    }
                    while (Vector2.Distance(tPos, pos) <= 10.0f);
                    Vars.teams.enemy.castles.Create(Cms.GetEntity("EnemyCastle0"), pos);
                    progress = 0.0f;
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
}