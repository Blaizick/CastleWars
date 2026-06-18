using System;
using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public class Mender : Building
    {
        [NonSerialized] public float progress;

        public override void Update()
        {
            progress += Time.deltaTime / cmsEntity.GetComponent<CmsReloadTimeComp>().reloadTime;

            if (progress >= 1.0f)
            {
                var hits = Physics2D.OverlapCircleAll(transform.position, cmsEntity.GetComponent<CmsRangeComp>().range);
                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent(out HealthComp h) && 
                        hit.TryGetComponent(out TeamComp t) && 
                        t.team.IsAlly(teamC.team))
                    {
                        h.Heal(cmsEntity.GetComponent<CmsHealComp>().heal);
                    }
                }

                var inst = Instantiate(cmsEntity.GetComponent<CmsMenderCirclePfbComp>().pfb, transform.position, Quaternion.identity, transform);
                Vars.restart.destroyOnRestart.Add(inst.gameObject);
                inst.cmsEntity = cmsEntity;
                inst.Init();

                progress = 0.0f;
            }

            base.Update();
        }
    }

    [Serializable]
    public class CmsHealComp : CmsComponent
    {
        public float heal;
    }

    [Serializable]
    public class CmsRangeComp : CmsComponent
    {
        public float range;
    }

    [Serializable]
    public class CmsReloadTimeComp : CmsComponent
    {
        public float reloadTime;
    }

    [Serializable]
    public class CmsMenderCirclePfbComp : CmsComponent
    {
        public MenderCircle pfb;
    }
}