using UnityEngine;

namespace FrameWork.UI
{
    public class UIApplyInverseSafeArea : MonoBehaviour
    {
        [SerializeField] private float _leftOffset;
        [SerializeField] private float _rightOffset;

        private RectTransform _rect;
        private Rect _lastSafeArea = Rect.zero;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            ApplyInverseSafeArea();
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea)
            {
                ApplyInverseSafeArea();
            }
        }

        private void ApplyInverseSafeArea()
        {
            var safeArea = Screen.safeArea;

            float safeLeft = safeArea.xMin;
            float safeRight = Screen.width - safeArea.xMax;

            _rect.offsetMin = new Vector2(_leftOffset + safeLeft, _rect.offsetMin.y);
            _rect.offsetMin = new Vector2(-_rightOffset - safeRight, _rect.offsetMin.y);

            _lastSafeArea = safeArea;
        }
    }
}