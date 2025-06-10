using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.GameSettings
{
	[AddComponentMenu("GameSettings/UI/Brightness Slider")]
	public class BrightnessSlider : MonoBehaviour
    {
		[SerializeField] private Slider targetElement;

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
					Debug.LogError("[BrightnessSlider] Slider 컴포넌트를 찾을 수 없습니다.", gameObject);
#endif
					return;
				}
			}

			targetElement.value = GameSettingsManager.ScreenBrightness;
			targetElement.onValueChanged.AddListener(OnValueChange);
		}

		private void OnValueChange(float value)
		{
			GameSettingsManager.ScreenBrightness = value;
		}
	}
}
