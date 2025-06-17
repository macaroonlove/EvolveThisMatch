using FrameWork.GameSettings;
using UnityEngine;

namespace FrameWork.UI
{
    public class SpriteRevolution : MonoBehaviour
    {
        public Vector3 offset;

        private void Start()
        {
            OnResolutionChange();

            GameSettingsManager.ResolutionChanged += OnResolutionChange;   
        }

        private void OnDestroy()
        {
            GameSettingsManager.ResolutionChanged -= OnResolutionChange;
        }

        private void OnResolutionChange()
        {
            Vector3 worldPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, Camera.main.nearClipPlane));
            worldPos.z = 0;

            transform.position = worldPos + offset;
        }
    }
}