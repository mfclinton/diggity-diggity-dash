using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Core.Utils
{
    public static class UIUtils
    {
        public static IEnumerator FadePanel(CanvasGroup panel, bool fadeIn = true, float duration = 0.3f, AnimationCurve scaleCurve = null)
        {
            // Setup
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            
            // Initial State
            float startT = fadeIn ? 0f : 1f;
            float endT = fadeIn ? 1f : 0f;

            panel.alpha = startT;
            
            // Animation
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float normalizedTime = Mathf.Clamp01(elapsed / duration);
                float t = Mathf.Lerp(startT, endT, normalizedTime);
                
                panel.alpha = t;
                
                if (scaleCurve != null)
                {
                    float curveValue = scaleCurve.Evaluate(t);
                    rectTransform.localScale = curveValue * Vector3.one;
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Final State
            panel.alpha = endT;
            if (scaleCurve != null)
                rectTransform.localScale = scaleCurve.Evaluate(endT) * Vector3.one;
        }

        public static IEnumerator AnimateUIElementIn(CanvasGroup element, float duration = 0.3f)
        {
            // Initial State
            element.alpha = 0f;
            
            // Animation
            float elapsed = 0f;
            while (elapsed < duration)
            {
                element.alpha = Mathf.Clamp01(elapsed / duration);

                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Final State
            element.alpha = 1f;
        }

        public static T CreateUIRow<T>(Transform parent, GameObject prefab) where T : Component
        {
            GameObject rowObj = Object.Instantiate(prefab, parent);
            return rowObj.GetComponent<T>();
        }

        public static void ClearUIElements<T>(System.Collections.Generic.List<T> elements) where T : Component
        {
            foreach (var element in elements)
                if (element != null)
                    Object.Destroy(element.gameObject);
            elements.Clear();
        }
    }
}
