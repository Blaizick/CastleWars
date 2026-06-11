

using System;
using UnityEngine;

namespace Proj21
{
    public class PlayerCastle : Castle
    {
        [NonSerialized] public Vector2 movePosition;
        [NonSerialized] public bool moving;

        [NonSerialized] public float progress;

        public SpriteRenderer selectSprite;
        public SpriteRenderer pathEndSprite;
        public LineRenderer pathLineRenderer;

        public override void Init()
        {
            base.Init();
            
            healthC.onDie.AddListener(() =>
            {
                Vars.ui.loseScreenRoot.SetActive(true);
            });

            selectSprite.transform.position = CenterPosition;
        }

        public override void Update()
        {
            progress += Time.deltaTime;
            if (progress >= 1.0f)
            {
                Vars.items.Add(new ItemStack(Items.Essence, 5));
                progress = 0.0f;
            }

            pathEndSprite.transform.position = movePosition;
            pathLineRenderer.useWorldSpace = true;
            pathLineRenderer.positionCount = 2;
            pathLineRenderer.SetPositions(new Vector3[]{CenterPosition, movePosition,});
            if (moving)
            {
                rb.linearVelocity = (movePosition - CenterPosition).normalized * cmsEntity.GetComponent<CmsSpeedComp>().speed;
                if (Vector2.Distance(CenterPosition, movePosition) <= cmsEntity.GetComponent<CmsSpeedComp>().speed * Time.fixedDeltaTime + Mathf.Epsilon)
                {
                    moving = false;
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }

            base.Update();
        }

        public void MoveTo(Vector2 position)
        {
            movePosition = position;
            moving = true;
        }
    }
}