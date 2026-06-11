
using System;
using DG.Tweening;
using UnityEngine;

namespace Proj21
{
    public class Collector : Building
    {
        [NonSerialized] public float progress;

        public override void Update()
        {
            progress += Time.deltaTime / 2.0f;
            if (progress >= 1.0f)
            {
                Vars.items.Add(new ItemStack(Items.Essence, 5));
                progress = 0.0f;
                transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.0f), 0.5f);
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