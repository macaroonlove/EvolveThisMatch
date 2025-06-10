using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.GameSettings
{
    [AddComponentMenu("GameSettings/UI/DisplayName Visible Toggle")]
    public class DisplayNameVisibleToggle : MonoBehaviour
    {
        [SerializeField] private Toggle targetElement;

        private void Reset()
        {
            targetElement = GetComponentInChildren<Toggle>();
        }

        private void Start()
        {
            if (targetElement == null)
            {
                targetElement = GetComponentInChildren<Toggle>();
                if (targetElement == null)
                {
#if UNITY_EDITOR
                    Debug.LogError("[DisplayNameVisibleToggle] Toggle 컴포넌트를 찾을 수 없습니다.", gameObject);
#endif
                    return;
                }
            }

            targetElement.isOn = GameSettingsManager.DisplayNameVisible;
            targetElement.onValueChanged.AddListener(OnValueChange);
        }

        private void OnValueChange(bool isOn)
        {
            GameSettingsManager.DisplayNameVisible = isOn;
        }
    }
}