using System;
using TMPro;
using UnityEngine;

namespace GUI
{
    /// A MonoBehaviour which scrolls text over time.
    /// Can only scroll one text object at a time.
    public class TextScroller : MonoBehaviour
    {
        private const float CharsPerSecond = 30;
        private TextMeshProUGUI textObject;
        private string targetText;
        private Action callback;
        private bool scrolling;
        private float lastCharacterUpdate;
        private int nextCharIndex;

        /// Begins scrolling the given text. Instantly finishes any previous scroll.
        /// Invokes the provided callback, if any, when finished.
        public void ScrollText(TextMeshProUGUI textObject, string targetText, Action finishedCallback = null)
        {
            if (scrolling)
            {
                this.textObject.text = this.targetText;
                this.callback?.Invoke();
            }

            this.textObject = textObject;
            this.targetText = targetText;
            this.callback = finishedCallback;
            this.textObject.text = "";
            nextCharIndex = 0;
            scrolling = true;
        }

        /// Instantly finishes the current text scroll.
        public void FinishScroll()
        {
            if (!scrolling)
            {
                Debug.LogWarning("No scroll in progress.");
                return;
            }

            textObject.text = targetText;
            scrolling = false;
            callback?.Invoke();
        }

        private void Update()
        {
            if (!scrolling) return;

            if (Time.time - lastCharacterUpdate > (1 / CharsPerSecond))
            {
                lastCharacterUpdate = Time.time;
                textObject.text += targetText[nextCharIndex];
                nextCharIndex++;
                if (nextCharIndex == targetText.Length)
                {
                    scrolling = false;
                    callback?.Invoke();
                }
            }
        }
    }
}
