using UnityEngine;

namespace FrameWork.UI
{
    public class SpriteRevolution : MonoBehaviour
    {
        public bool isRight;
        public Vector3 offset;

        private Rect _lastSafeArea = Rect.zero;

        private void Awake()
        {
            OnResolutionChange();
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea)
            {
                OnResolutionChange();
            }
        }

        private void OnResolutionChange()
        {
            Rect safeArea = Screen.safeArea;

            float left = safeArea.xMin / Screen.width;
            float right = safeArea.xMax / Screen.width;

            float x = isRight ? right : left;
            Vector3 worldPos = Camera.main.ViewportToWorldPoint(new Vector3(x, 0.5f, Camera.main.nearClipPlane));
            worldPos.z = 0;

            transform.position = worldPos + offset;

            _lastSafeArea = safeArea;
        }
    }
}