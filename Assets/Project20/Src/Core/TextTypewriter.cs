using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Proj21
{
    public class TextTypewriter : MonoBehaviour, IPointerClickHandler
    {
        public TMP_Text targetText; 

        public float charsPerSec = 20.0f;

        public Coroutine writeCoroutine = null; 

        [NonSerialized] public string writingText;
        [NonSerialized] public bool skipRequested = false;

        [NonSerialized] public bool skipOnClick = false;
        [NonSerialized] public bool clicked = false;

        public IEnumerator WaitForClick()
        {
            clicked = false;
            while (!clicked)
            {
                yield return null;
            }
        }

        public Coroutine WriteCoroutine(string text)
        {
            if (writeCoroutine != null)
            {
                StopCoroutine(writeCoroutine);
            }
            writeCoroutine = StartCoroutine(_WriteCoroutine(text));
            return writeCoroutine;
        }
        private IEnumerator _WriteCoroutine(string text)
        {
            skipRequested = false;
            writingText = text;
            targetText.text = text;
            int c = 0;
            while (c < text.Length)
            {
                if (skipRequested)
                {
                    skipRequested = false;
                    yield break;
                }
                targetText.maxVisibleCharacters = ++c;           
                yield return new WaitForSeconds(1.0f / charsPerSec);         
            }
        }

        public void Skip()
        {
            if (writeCoroutine != null)
            {
                targetText.text = writingText;
                targetText.maxVisibleCharacters = int.MaxValue;
                skipRequested = true;
                writeCoroutine = null;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            clicked = true;
            if (skipOnClick)
            {
                Skip();
            }
        }
    }
}