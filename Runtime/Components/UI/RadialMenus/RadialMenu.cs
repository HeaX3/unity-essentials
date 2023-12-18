using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Essentials
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class RadialMenu : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private RectTransform _rectTransform;

        private void OnEnable()
        {
            UpdateLayout();
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateLayout();
        }

        public void UpdateLayout()
        {
            var children = _rectTransform.GetChildren().OfType<RectTransform>().ToList();
            for (var i = 0; i < children.Count; i++)
            {
                var child = children[i];
                var position = (0.75f - i / (float)children.Count) * 2 * Mathf.PI;
                var x = (Mathf.Cos(position) + 1) / 2;
                var y = (Mathf.Sin(position) + 1) / 2;
                child.anchorMin = new Vector2(x, y);
                child.anchorMax = new Vector2(x, y);
                child.anchoredPosition = Vector2.zero;
            }
        }

        private void OnValidate()
        {
            if (!_rectTransform || _rectTransform.gameObject != gameObject)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            if (gameObject.activeInHierarchy) StartCoroutine(DelayRoutine(UpdateLayout));
        }

        private IEnumerator DelayRoutine(Action callback)
        {
            yield return new WaitForEndOfFrame();
            callback();
        }
    }
}