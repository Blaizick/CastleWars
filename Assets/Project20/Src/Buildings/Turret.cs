using System;
using Blaze.Runtime.Cms;
using DG.Tweening;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Proj21
{
    public class Turret : Building
    {
        public Transform towerRootTr;
        public Transform baseRootTr;
        public Transform shootPositionTr;
        [NonSerialized] public Transform targetTr;
        [NonSerialized] public float reloadProgress;
        public Tween recoilTween = null;
        [NonSerialized] public CmsEntity projectile = null;
        [NonSerialized] public int projectileCount;

        public override void Update()
        {
            reloadProgress += Time.deltaTime * cmsEntity.GetComponent<CmsAttackSpeedComp>().attackSpeed;
            
            CmsEntity projectile = cmsEntity.GetComponent<CmsProjectileComp>().projectile.GetCmsEntity();
            float range = projectile.GetComponent<CmsSpeedComp>().speed * projectile.GetComponent<CmsLifetimeComp>().lifetime - 1.0f;            

            var hits = Physics2D.OverlapCircleAll(transform.position, range);
            targetTr = null;
            float minDst = 0.0f;
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out HealthComp h) && 
                    hit.TryGetComponent(out TeamComp t) && 
                    t.team.IsEnemy(teamC.team))
                {
                    float dst = Vector2.Distance(transform.position, hit.transform.position);
                    if (targetTr == null || dst < minDst)
                    {
                        minDst = dst;
                        targetTr = hit.transform;
                    }
                }
            }
            
            if (targetTr)
            {
                Vector2 targetPos = targetTr.position;
                if (targetTr.TryGetComponent<IPositionProvider>(out var _p))
                {
                    targetPos = _p.GetPosition();
                }
                Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
                float rotOffset = 0.0f;
                if (cmsEntity.HasComponent<CmsRotationOffsetComp>())
                {
                    rotOffset = cmsEntity.GetComponent<CmsRotationOffsetComp>().rotationOffset;
                }
                float targetRot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + rotOffset;
                towerRootTr.transform.rotation = Quaternion.RotateTowards(towerRootTr.rotation, 
                    Quaternion.Euler(0.0f, 0.0f, targetRot), 
                    cmsEntity.GetComponent<CmsRotationSpeedComp>().rotationSpeed * Time.deltaTime);

                bool ignoreProjectileCount = teamC.team != Vars.teams.ally;
                if (!ignoreProjectileCount)
                {
                    if (projectileCount <= 0)
                    {
                        var projCost = projectile.GetComponent<CmsProjectileCostComp>();
                        var cost = projCost.itemCost.AsItemStack();
                        if (Vars.items.Has(cost))
                        {
                            Vars.items.Remove(cost);
                            this.projectile = projectile;
                            projectileCount += projCost.count;
                        }
                    }
                }
                if (reloadProgress >= 1.0f && 
                    Mathf.Abs(Mathf.DeltaAngle(towerRootTr.transform.eulerAngles.z, targetRot)) <= 5.0f && 
                    (projectileCount > 0 || ignoreProjectileCount))
                {
                    if (--projectileCount <= 0)
                    {
                        this.projectile = null;
                    }
                    int count = 1;
                    if (projectile.HasComponent<CmsProjectileCountComp>())
                    {
                        count = projectile.GetComponent<CmsProjectileCountComp>().projectilesCount;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        float maxRandomOffset = projectile.GetComponent<CmsRandomOffsetComp>().randomOffset / 2;
                        float rot = (towerRootTr.eulerAngles.z + UnityEngine.Random.Range(-maxRandomOffset, maxRandomOffset) - rotOffset) * Mathf.Deg2Rad;
                        Vector2 dir2 = new Vector2(Mathf.Cos(rot), Mathf.Sin(rot));

                        var inst = Instantiate(projectile.GetComponent<CmsPfbComp>().pfb, shootPositionTr.transform.position, Quaternion.identity).GetComponent<Projectile>();
                        inst.Set(teamC.team, projectile, dir2);
                        inst.Init();
                        reloadProgress = 0.0f;

                        CmsRecoilComp recoilComp = cmsEntity.GetComponent<CmsRecoilComp>();
                        var punchDeg = towerRootTr.eulerAngles.z + rotOffset;
                        var punch = new Vector2(Mathf.Cos(punchDeg * Mathf.Deg2Rad), Mathf.Sin(punchDeg * Mathf.Deg2Rad)) * recoilComp.recoildStreangth;
                        if (recoilTween != null)
                        {
                            recoilTween.Complete();
                        }
                        recoilTween = towerRootTr.DOPunchPosition(punch, recoilComp.recoilDuration).OnComplete(() =>
                        {
                            recoilTween = null;
                        });
                    }
                }
            }

            base.Update();
        }

        public override void OnDestroy()
        {
            towerRootTr.DOKill();

            base.OnDestroy();
        }
    }

    [Serializable]
    public class CmsProjectileComp : CmsComponent
    {
        public CmsEntityPfb projectile;        
    }

    [Serializable]
    public class CmsAttackSpeedComp : CmsComponent
    {
        //In attacks/second
        public float attackSpeed;
    }

    [Serializable]
    public class CmsRotationSpeedComp : CmsComponent
    {
        public float rotationSpeed;
    }

    [Serializable]
    public class CmsSpeedComp : CmsComponent
    {
        public float speed;
    }

    [Serializable]
    public class CmsLifetimeComp : CmsComponent
    {
        public float lifetime;
    }

    [Serializable]
    public class CmsRotationOffsetComp : CmsComponent
    {
        public float rotationOffset;
    }

    [Serializable]
    public class CmsRecoilComp : CmsComponent
    {
        public float recoildStreangth;
        public float recoilDuration;
    }

    [Serializable]
    public class CmsProjectileCountComp : CmsComponent
    {
        public int projectilesCount;
    }

    [Serializable]
    public class CmsRandomOffsetComp : CmsComponent
    {
        public float randomOffset;
    }

    [Serializable]
    public class CmsProjectileCostComp : CmsComponent
    {
        public CmsItemStack itemCost;
        public int count;
    }
}