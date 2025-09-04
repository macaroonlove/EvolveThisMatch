using UnityEngine;

namespace FrameWork.UI
{
    public class UIApplySafeArea : MonoBehaviour
    {
        private void Start()
        {
            var rect = this.GetComponent<RectTransform>();

            var minAnchor = Screen.safeArea.min;
            var maxAnchor = Screen.safeArea.max;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;

            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            rect.anchorMin = minAnchor;
            rect.anchorMax = maxAnchor;
        }
    }
}