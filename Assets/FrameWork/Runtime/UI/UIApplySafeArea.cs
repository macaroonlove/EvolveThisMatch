using UnityEngine;

namespace FrameWork.UI
{
    public class UIApplySafeArea : MonoBehaviour
    {
        private RectTransform _rect;
        private Rect _lastSafeArea = Rect.zero;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea)
            {
                ApplySafeArea();
            }
        }

        private void ApplySafeArea()
        {
            var safeArea = Screen.safeArea;

            var minAnchor = safeArea.min;
            var maxAnchor = safeArea.max;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;

            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            _rect.anchorMin = minAnchor;
            _rect.anchorMax = maxAnchor;

            _lastSafeArea = safeArea;
        }
    }
}