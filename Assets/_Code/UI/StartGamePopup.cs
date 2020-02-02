using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Code.UI {
    public class StartGamePopup : MonoBehaviour {
        [SerializeField, Range(0.01f, 5)] float speed;
        [SerializeField] TextMeshProUGUI[] texts;
        [SerializeField] Image image;
        [SerializeField] AnimationCurve fadingCurve;

        Coroutine fadingCoroutine;

        [ContextMenu("Start")]
        public void StartFading() {
            fadingCoroutine = StartCoroutine(FadingCoroutine());
        }

        [ContextMenu("Stop")]
        public void StopFading() {
            if(fadingCoroutine != null)
                StopCoroutine(FadingCoroutine());
        }

        public void SetActive(bool value) {
            gameObject.SetActive(value);
            StopFading();
        }

        IEnumerator FadingCoroutine() {
            var time = 0f;
            while (true) {
                time += Time.deltaTime * speed;
                if (time >= 1)
                    time = 0;
                FadeTexts(time);
                FadeIcon(time);
                yield return null;
            }
        }

        void FadeIcon(float time) {
            var color = image.color;
            color.a = fadingCurve.Evaluate(time);
            image.color = color;
        }

        void FadeTexts(float time) {
            foreach (var text in texts) {
                var color = text.color;
                color.a = fadingCurve.Evaluate(time);
                text.color = color;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Collect fading objects")]
        void CollectFadingObjects() {
            UnityEditor.Undo.RecordObject(this, "CollectFadingObjects");
            texts = GetComponentsInChildren<TextMeshProUGUI>();
            image = GetComponentInChildren<Image>();
        }
        #endif
    
    }
}
