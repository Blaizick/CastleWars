using System;
using Blaze.Runtime.Cms;
using UnityEngine;
using UnityEngine.Events;

namespace Proj21
{
    public class HealthComp : MonoBehaviour
    {
        [NonSerialized] public float maxHealth;
        [NonSerialized] public float health;

        public UnityEvent onDie = new();
        public UnityEvent onDamaged = new();
        public UnityEvent onHeal = new();

        public virtual void SetFromCmsEntity(CmsEntity entity)
        {
            Set(entity.GetComponent<CmsHealthComp>().maxHealth);
        }
        public virtual void Set(float maxHealth)
        {
            this.maxHealth = maxHealth;
        }

        public virtual void Init()
        {
            health = maxHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            health = Mathf.Clamp(health - damage, 0, maxHealth);
            onDamaged.Invoke();
            if (health <= 0)
            {
                onDie.Invoke();
            }
        }

        public virtual void Heal(float heal)
        {
            health = Mathf.Clamp(health + heal, 0, maxHealth);
            onHeal.Invoke();
        }
        
        public virtual bool CanBeDamaged(object source)
        {
            return true;
        }
    }
}