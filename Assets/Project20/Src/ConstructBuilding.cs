

using System;
using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public class ConstructBuilding : Building
    {
        [NonSerialized] public float progress;
        public SpriteRenderer spriteRenderer;
        public MaterialPropertyBlock propertyBlock;

        public override void Init()
        {
            propertyBlock = new();

            spriteRenderer.sprite = cmsEntity.GetComponent<CmsSpriteComp>().sprite;
            
            spriteRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetVector("_uvOffset", new Vector2(UnityEngine.Random.Range(-10000, 10000), UnityEngine.Random.Range(-10000, 10000)));
            propertyBlock.SetFloat("_noiseScale", 200.0f / Size);

            spriteRenderer.SetPropertyBlock(propertyBlock);

            base.Init();
        }

        public override void Update()
        {
            progress += Time.deltaTime / cmsEntity.GetComponent<CmsConstructTimeComp>().constructTime;

            spriteRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetFloat("_time", progress);

            spriteRenderer.SetPropertyBlock(propertyBlock);

            if (progress >= 1.0f)
            {
                castle.FinishCostructingBuilding(this);
            }

            base.Update();
        }
    }

    [Serializable]
    public class CmsConstructTimeComp : CmsComponent
    {
        public float constructTime;
    }
}