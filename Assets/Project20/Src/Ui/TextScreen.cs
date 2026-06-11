using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Proj21
{
    public class TextScreen : MonoBehaviour
    {
        public TextTypewriter typewriter;
        public GameObject root;
        public CanvasGroup canvasGroup;

        public void Init()
        {
            root.SetActive(false);
        }

        public IEnumerator WriteTextCoroutine(string text)
        {
            yield return typewriter.WriteCoroutine(text);
        }

        public IEnumerator WaitUntilClick()
        {
            clicked = false;
            while (!clicked)
            {
                yield return null;                
            }
        }

        [NonSerialized] public bool clicked = false;

        public void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (typewriter.writeCoroutine == null)
                {
                    if (!clicked)
                    {
                        clicked = true;
                    }
                }
                else
                {
                    typewriter.Skip();
                }    
            }
        }

        public IEnumerator HideCoroutine()
        {
            yield return canvasGroup.DOFade(0.0f, 0.5f).WaitForCompletion();
            root.SetActive(false);
        }

        public IEnumerator ShowCoroutine()
        {
            root.SetActive(true);
            canvasGroup.alpha = 0.0f;
            yield return canvasGroup.DOFade(1.0f, 0.5f).WaitForCompletion();
        }
    }
}