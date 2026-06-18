
using System;
using Blaze.Runtime.Cms;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Proj21
{
    [Serializable]
    public class CmsIngradietStackComp : CmsComponent
    {
        public CmsItemStack ingradientStack;
    }
    [Serializable]
    public class CmsProductStackComp : CmsComponent
    {
        public CmsItemStack productStack;
    }
    [Serializable]
    public class CmsCraftTimeComp : CmsComponent
    {
        public float craftTime;
    }

    [Serializable]
    public class CmsRecipeComp : CmsComponent
    {
        public CmsEntityPfb recipe;
    }

    public class Collector : Building
    {
        [NonSerialized] public float progress;
        [NonSerialized] public bool crafting;

        public override void Update()
        {
            CmsEntity recipe = cmsEntity.GetComponent<CmsRecipeComp>().recipe.GetCmsEntity();
            if (!crafting)
            {
                bool can = true;
                foreach (var i in recipe.GetAllComponentsOfType<CmsIngradietStackComp>())
                {
                    var stack = i.ingradientStack.AsItemStack();
                    if (!Vars.items.Has(stack))
                    {
                        can = false;
                        break;
                    }
                }
                if (can)
                {
                    foreach (var i in recipe.GetAllComponentsOfType<CmsIngradietStackComp>())
                    {
                        Vars.items.Remove(i.ingradientStack.AsItemStack());
                    }
                    crafting = true;
                }
            }
            if (crafting)
            {
                progress += Time.deltaTime / recipe.GetComponent<CmsCraftTimeComp>().craftTime;
                if (progress >= 1.0f)
                {
                    foreach (var i in recipe.GetAllComponentsOfType<CmsProductStackComp>())
                    {
                        Vars.items.Add(i.productStack.AsItemStack());
                    }
                    progress = 0.0f;
                    transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.0f), 0.5f);
                    crafting = false;
                }
            }
            
            base.Update();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            transform.DOKill();
        }
    }
}