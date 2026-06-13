using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Proj21
{
    public class SceneTransitionScreen : MonoBehaviour
    {
        public GameObject root;
        public Material material;
        public Tween tween = null;

        public void PlayShowAnim(UnityAction onComplete)
        {
            root.SetActive(true);
            if (tween != null)
            {
                tween.Complete();
            }
            material.SetFloat("_time", 0.0f);
            tween = material.DOFloat(1.0f, "_time", 0.5f).OnComplete(() =>
            {
                tween = null;
                onComplete?.Invoke();
                material.DOKill();
            });
        }
        public void PlayHideAnim()
        {
            root.SetActive(true);
            if (tween != null)
            {
                tween.Complete();
            }
            material.SetFloat("_time", 1.0f);
            tween = material.DOFloat(0.0f, "_time", 0.5f).OnComplete(() =>
            {
                tween = null;
                root.SetActive(false);
            });
        }
    }
}