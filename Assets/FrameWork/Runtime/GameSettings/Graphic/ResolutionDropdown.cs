using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FrameWork.GameSettings
{
    [AddComponentMenu("GameSettings/UI/Resolution Dropdown")]
    public class ResolutionDropdown : MonoBehaviour
    {
		[SerializeField] private TMP_Dropdown targetElement;

		private void Reset()
		{
			targetElement = GetComponentInChildren<TMP_Dropdown>();
		}

		private void Awake()
		{
			if (targetElement == null)
			{
				targetElement = GetComponentInChildren<TMP_Dropdown>();
				if (targetElement == null)
				{
#if UNITY_EDITOR
					Debug.LogError("[ResolutionDropdown] TextMeshPro Dropdown 컴포넌트를 찾을 수 없습니다.", gameObject);
#endif
					return;
				}
			}
		}

		private void Start()
		{
			GameSettingsManager.ResolutionChanged += RefreshControlDelayed;
		}

		private void OnDestroy()
		{
			GameSettingsManager.ResolutionChanged -= RefreshControlDelayed;
		}

		private void OnEnable()
		{
			RefreshControl();
		}

		private void RefreshControlDelayed()
		{
			StartCoroutine(RefreshControlSequence());
		}

		private IEnumerator RefreshControlSequence()
		{
			// 해상도 전환은 프레임 끝에서 발생하므로 값을 새로 고치기 전에 기다려야 한다.
			yield return new WaitForFixedUpdate();
			RefreshControl();
		}

		private void RefreshControl()
		{
			if (targetElement != null)
			{
				targetElement.ClearOptions();
				targetElement.onValueChanged.RemoveAllListeners();

				List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
				foreach (string resolution in GameSettingsManager.ScreenResolutions)
				{
					options.Add(new TMP_Dropdown.OptionData(resolution));
				}

				targetElement.AddOptions(options);
				targetElement.value = GameSettingsManager.ScreenResolutionIndex;
				targetElement.onValueChanged.AddListener(OnValueChange);
			}
		}

		private void OnValueChange(int idx)
		{
			GameSettingsManager.ScreenResolutionIndex = idx;
		}
	}
}
