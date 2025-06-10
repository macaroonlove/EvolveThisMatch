using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.GameSettings
{
    [AddComponentMenu("GameSettings/UI/Quality Toggle")]
    public class QualityToggle : MonoBehaviour
    {
		[SerializeField] private Toggle[] targetElements;

		private void Reset()
		{
			targetElements = GetComponentsInChildren<Toggle>();
		}

		private void Start()
		{
			if (targetElements.Length == 0)
			{
				targetElements = GetComponentsInChildren<Toggle>();
				if (targetElements.Length == 0)
				{
#if UNITY_EDITOR
					Debug.LogError("[QualityToggle] Toggle 컴포넌트를 찾을 수 없습니다.", gameObject);
#endif
					return;
				}
			}

			targetElements[GameSettingsManager.GFXQualityLevelIndex].isOn = true;

			for (int i = 0; i < targetElements.Length; i++)
			{
				int index = i;
				targetElements[i].onValueChanged.AddListener((isOn) => OnValueChange(isOn, index));
			}
		}

		private void OnValueChange(bool isOn, int index)
		{
			if (isOn)
			{
				switch (index)
				{
					case 0:
						GameSettingsManager.GFXQualityLevelIndex = 2;
						break;
					case 1:
						GameSettingsManager.GFXQualityLevelIndex = 1;
						break;
					case 2:
						GameSettingsManager.GFXQualityLevelIndex = 0;
						break;
				}
			}
		}
	}
}
