using System;
using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public class MenderCircle : MonoBehaviour
    {
        [NonSerialized] public float progress;
        public CmsEntity cmsEntity;
        public SpriteRenderer spriteRenderer;

        public void Init()
        {
            
        }

        public void Update()
        {
            progress += Time.deltaTime / cmsEntity.GetComponent<CmsMenderCircleComp>().lifetime;            
            float s = Mathf.Lerp(cmsEntity.GetComponent<CmsMenderCircleComp>().minScale, 
                cmsEntity.GetComponent<CmsMenderCircleComp>().maxScale,
                progress);
            transform.localScale = new Vector3(s, s, 1.0f);
            if (progress <= 0.3f)
            {
                float t = progress / 0.3f;
                spriteRenderer.color = Color.Lerp(
                    cmsEntity.GetComponent<CmsMenderCircleComp>().col0, 
                    cmsEntity.GetComponent<CmsMenderCircleComp>().col1,
                    t);
            }
            if (progress >= 0.7f)
            {
                float t = (progress - 0.7f) / 0.3f;
                spriteRenderer.color = Color.Lerp(
                    cmsEntity.GetComponent<CmsMenderCircleComp>().col1, 
                    cmsEntity.GetComponent<CmsMenderCircleComp>().col0,
                    t);
            }
            if (progress >= 1.0f)
            {
                Vars.restart.destroyOnRestart.Remove(gameObject);
                Destroy(gameObject);
            }
        }
    }

    [Serializable]
    public class CmsMenderCircleComp : CmsComponent
    {
        public float lifetime;
        public float minScale;
        public float maxScale;
        public Color col0;
        public Color col1;
    }
}