

using System;
using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    [Serializable]
    public class CmsItemStack
    {
        public CmsEntityPfb item;
        public int count;

        public ItemStack AsItemStack()
        {
            return new ItemStack(item.GetCmsEntity(), count);
        }
    }

    [Serializable]
    public class CmsBuildCostComp : CmsComponent
    {
        public CmsItemStack cost;
    }

    public class Building : MonoBehaviour
    {
        [NonSerialized] public Castle castle;
        [NonSerialized] public Vector2Int pos;

        public CmsEntity cmsEntity;

        public TeamComp teamC;
        public HealthComp healthC;

        public virtual void Init()
        {
            healthC.Set(cmsEntity.GetComponent<CmsHealthComp>().maxHealth);
            healthC.Init();
            healthC.onDie.AddListener(() => 
            {
                castle.DestroyBuilding(this);
                Vars.effects.CreateEffect(Vars.effects.buildingDestroyEffect, transform.position);
            });
        }

        public virtual void Update()
        {
            
        }

        public virtual void OnDestroy()
        {
        }

        public int Size => cmsEntity.GetComponent<CmsSquareSizeComp>().size;

        public RectInt GetRect()
        {
            return new RectInt(pos, new Vector2Int(Size, Size));
        }
    }

    [Serializable]
    public class CmsHealthComp : CmsComponent
    {
        public float maxHealth;
    }
}