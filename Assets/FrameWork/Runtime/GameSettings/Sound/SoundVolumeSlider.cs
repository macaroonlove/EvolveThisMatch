using FrameWork.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.GameSettings
{
    [AddComponentMenu("GameSettings/UI/Sound Volume Slider")]
    public class SoundVolumeSlider : MonoBehaviour
    {
        [SerializeField] private Slider targetElement;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Audio.AudioType volumeType = Audio.AudioType.Master;

        private void Reset()
        {
            targetElement = GetComponentInChildren<Slider>();
        }

        private void Start()
        {
            if (targetElement == null)
            {
                targetElement = GetComponentInChildren<Slider>();
                if (targetElement == null)
                {
#if UNITY_EDITOR
                    Debug.LogError("[SoundVolumeSlider] Slider 컴포넌트를 찾을 수 없습니다.", gameObject);
#endif
                    return;
                }
            }

            targetElement.value = GameSettingsManager.GetSoundVolume(volumeType);
            targetElement.onValueChanged.AddListener(OnValueChange);

            if (text != null)
            {
                text.text = (targetElement.value * 100).ToString("F0");
            }
        }

        private void OnValueChange(float value)
        {
            GameSettingsManager.SetSoundVolume(volumeType, value);
            if (text != null) text.text = (value * 100).ToString("F0");
        }
    }
}
