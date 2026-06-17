using System;
using Blaze.Runtime.Cms;
using UnityEngine;
using UnityEngine.Events;

namespace Proj21
{
    public class BaseConstructCastleOperator
    {
        public AppearCastle castle;

        public virtual void Init()
        {
            
        }

        public virtual void Update()
        {
            
        }
    }

    public class ConstructCastleOperator : BaseConstructCastleOperator
    {
        public float progress;
        public MaterialPropertyBlock propertyBlock;
        public UnityEvent<Castle> onAppear = new();

        public override void Init()
        {
            base.Init();
        
            propertyBlock = new();

            foreach (var i in castle.tiles)
            {
                i.spriteRenderer.material = Resources.Load<Material>("Mat/AppearMaterial");
                i.spriteRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetVector("_uvOffset", new Vector2(UnityEngine.Random.Range(-10000.0f, 10000.0f), UnityEngine.Random.Range(-10000.0f, 10000.0f)));
                propertyBlock.SetVector("_noiseScale", NoiseUtils.GetScale(Vector2.one, i.spriteRenderer.sprite.texture));
                i.spriteRenderer.SetPropertyBlock(propertyBlock);
            }

            castle.outlineSpriteRenderer.color = Color.white;

            castle.outlineSpriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetVector("_uvOffset", new Vector2(UnityEngine.Random.Range(-10000.0f, 10000.0f), UnityEngine.Random.Range(-10000.0f, 10000.0f)));
            propertyBlock.SetVector("_noiseScale", NoiseUtils.GetScale(new Vector2(castle.size.x, castle.size.y), castle.outlineSpriteRenderer.sprite.texture));
            // propertyBlock.SetVector("_noiseScale", Vector2.one / new Vector2(castle.size.x, castle.size.y));
            propertyBlock.SetColor("_mainCol", castle.cmsEntity.GetComponent<CmsOutlineColorComp>().outlineColor);
            castle.outlineSpriteRenderer.SetPropertyBlock(propertyBlock);
        }

        public override void Update()
        {
            progress += Time.deltaTime;

            foreach (var i in castle.tiles)
            {
                i.spriteRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat("_time", progress);
                i.spriteRenderer.SetPropertyBlock(propertyBlock);
            }

            castle.outlineSpriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_time", progress);
            castle.outlineSpriteRenderer.SetPropertyBlock(propertyBlock);

            if (progress >= 1.0f)
            {
                var cmsEnt = castle.cmsEntity;
                var team = castle.teamC.team;
                var pos = castle.transform.position;
                team.castles.Destroy(castle);
                var _castle = castle.teamC.team.castles.Create(castle.cmsEntity, castle.transform.position);
                onAppear.Invoke(_castle);
            }

            base.Update();
        }
    }
    
    public class DeconstructCastleOperator : BaseConstructCastleOperator
    {
        public float progress;
        public MaterialPropertyBlock propertyBlock;

        public override void Init()
        {
            base.Init();
        
            propertyBlock = new();

            foreach (var i in castle.tiles)
            {
                i.spriteRenderer.material = Resources.Load<Material>("Mat/AppearMaterial");
                i.spriteRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetVector("_uvOffset", new Vector2(UnityEngine.Random.Range(-10000.0f, 10000.0f), UnityEngine.Random.Range(-10000.0f, 10000.0f)));
                propertyBlock.SetVector("_noiseScale", NoiseUtils.GetScale(Vector2.one, i.spriteRenderer.sprite.texture));
                // propertyBlock.SetFloat("_noiseScale", 1.0f);
                propertyBlock.SetColor("_color", new Color(1.0f, 0.0f, 0.0f, 1.0f));
                i.spriteRenderer.SetPropertyBlock(propertyBlock);
            }

            castle.outlineSpriteRenderer.color = Color.white;

            castle.outlineSpriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetVector("_uvOffset", new Vector2(UnityEngine.Random.Range(-10000.0f, 10000.0f), UnityEngine.Random.Range(-10000.0f, 10000.0f)));
            // propertyBlock.SetFloat("_noiseScale", 1.0f / castle.size.x);
            propertyBlock.SetVector("_noiseScale", NoiseUtils.GetScale(new Vector2(castle.size.x, castle.size.y), castle.outlineSpriteRenderer.sprite.texture));
            propertyBlock.SetColor("_color", new Color(1.0f, 0.0f, 0.0f, 1.0f));
            propertyBlock.SetColor("_mainCol", castle.cmsEntity.GetComponent<CmsOutlineColorComp>().outlineColor);
            castle.outlineSpriteRenderer.SetPropertyBlock(propertyBlock);
        }

        public override void Update()
        {
            progress += Time.deltaTime;

            foreach (var i in castle.tiles)
            {
                i.spriteRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat("_time", 1.0f - progress);
                i.spriteRenderer.SetPropertyBlock(propertyBlock);
            }

            castle.outlineSpriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_time", 1.0f - progress);
            castle.outlineSpriteRenderer.SetPropertyBlock(propertyBlock);

            if (progress >= 1.0f)
            {
                castle.teamC.team.castles.Destroy(castle);
                // castle.teamC.team.castles.FinishConstructing(castle);
            }

            base.Update();
        }
    }

    public class AppearCastle : Castle
    {
        public BaseConstructCastleOperator _operator;

        public override void Init()
        {
            base.Init();

            _operator.castle = this;
            _operator.Init();
        }

        public override void Update()
        {
            base.Update();

            _operator.Update();
        }
    }

    public static class NoiseUtils
    {
        public static Vector2 GetScale(Vector2 objectSize, Vector2 textureSize)
        {
            return (Vector2.one / objectSize) * textureSize;
        }

        public static Vector2 GetScale(Vector2 objectSize, Texture2D texture)
        {
            return GetScale(objectSize, new Vector2(texture.width, texture.height));
        }
    }
}