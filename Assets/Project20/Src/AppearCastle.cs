using System;
using UnityEngine;
using UnityEngine.Events;

namespace Proj21
{
    public class AppearCastle : Castle
    {
        [NonSerialized] public float progress;
        [NonSerialized] public MaterialPropertyBlock propertyBlock;

        public UnityEvent<Castle> onAppear = new();

        public override void Init()
        {
            base.Init();

            propertyBlock = new();

            foreach (var i in tiles)
            {
                i.spriteRenderer.material = Resources.Load<Material>("Mat/AppearMaterial");
                i.spriteRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetVector("_uvOffset", new Vector2(UnityEngine.Random.Range(-10000.0f, 10000.0f), UnityEngine.Random.Range(-10000.0f, 10000.0f)));
                propertyBlock.SetFloat("_noiseScale", 10.0f);
                i.spriteRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        public override void Update()
        {
            base.Update();

            progress += Time.deltaTime;

            foreach (var i in tiles)
            {
                i.spriteRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat("_time", progress);
                i.spriteRenderer.SetPropertyBlock(propertyBlock);
            }

            if (progress >= 1.0f)
            {
                teamC.team.castles.FinishConstructing(this);
            }
        }
    }
}