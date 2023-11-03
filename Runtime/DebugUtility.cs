using UnityEngine;
using UnityEngine.UI;

namespace Essentials
{
    public class DebugUtility : MonoBehaviour
    {
        private static DebugUtility instance;

        [SerializeField] private Canvas _debugCanvas;
        [SerializeField]
        private RectTransform debugInfo;
        [SerializeField]
        private Text hintsText;
        [SerializeField]
        private CanvasGroup hintsGroup;
        [SerializeField]
        private bool drawInputs = false;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        private void OnDestroy()
        {
            if (instance == this) instance = null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                _debugCanvas.gameObject.SetActive(!_debugCanvas.gameObject.activeSelf);
            }
            if (hintsGroup.alpha > 0)
            {
                hintsGroup.alpha -= Time.deltaTime / 2;
            }
        }

        public static void showHint(string hint)
        {
            instance.hintsText.text = hint;
            instance.hintsGroup.alpha = 1;
        }

        public static GameObject DrawMarker(Vector3 position, float t = 10f)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.position = position;
            marker.transform.localScale = Vector3.one * 0.1f;
            Destroy(marker,t);
            return marker;
        }
    }
}
