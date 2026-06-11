

using System;
using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public class Projectile : MonoBehaviour
    {
        public Rigidbody2D rb;
        [NonSerialized] public Vector2 dir;
        [NonSerialized] public float progress;
        public CmsEntity cmsEntity;
        public TeamComp teamC;
        public SpriteRenderer spriteRenderer;

        public virtual void Set(Team team, CmsEntity cmsEntity, Vector2 dir)
        {
            teamC.Set(team);
            this.cmsEntity = cmsEntity;
            this.dir = dir;
        }

        public virtual void Init()
        {
            rb.linearVelocity = dir * cmsEntity.GetComponent<CmsSpeedComp>().speed;
            Vars.restart.destroyOnRestart.Add(gameObject);
        }

        public virtual void Update()
        {
            if (progress <= 0.1f)
            {
                float t = progress / 0.1f;
                Color col = spriteRenderer.color;
                spriteRenderer.color = new Color(col.r, col.g, col.b, Mathf.Lerp(0.2f, 1.0f, t));
            }
            if (progress >= 0.9f)
            {
                float t = (progress - 0.9f) / 0.1f;
                Color col = spriteRenderer.color;
                spriteRenderer.color = new Color(col.r, col.g, col.b, Mathf.Lerp(1.0f, 0.2f, t));
            }

            progress += Time.deltaTime / cmsEntity.GetComponent<CmsLifetimeComp>().lifetime;
            if (progress >= 1.0f)
            {
                Destroy(gameObject);
            }

            var hits = Physics2D.OverlapPointAll(transform.position);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out HealthComp h) && 
                    hit.TryGetComponent(out TeamComp t) && 
                    h.CanBeDamaged(this) && 
                    t.team.IsEnemy(teamC.team))
                {
                    h.TakeDamage(cmsEntity.GetComponent<CmsDamageComp>().damage);
                    Destroy(gameObject);
                }
            }            
        }

        public void OnDestroy()
        {
            Vars.restart.destroyOnRestart.Remove(gameObject);
        }
    }
    [Serializable]
    public class CmsDamageComp : CmsComponent
    {
        public float damage;
    }
}