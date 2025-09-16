using FrameWork.Sound;
//using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.GameSettings
{
	/// <summary>
	/// PlayerFrefs로 레지스트리에 저장될 설정들을 관리하는 매니저
	/// </summary>
	public static class GameSettingsManager
	{
		// ------------------------------------------------------------------------------------------------------------
		#region 설정 복원

		public static event System.Action OnRestoreComplete;

		/// <summary> 저장된 설정을 복원합니다. </summary>
		public static void RestoreSettings()
		{
			// 최대 FPS 설정 복원
			int maxFPSIndex = PlayerPrefs.GetInt("Settings.MaxFPS", FPSOptions.Length - 1);
			Application.targetFrameRate = FPSOptions[maxFPSIndex];
			
			// 퀄리티 설정 복원
			int q = PlayerPrefs.GetInt("Settings.Quality", QualitySettings.GetQualityLevel());
			QualitySettings.SetQualityLevel(q);

			// 소리 설정 복원
			var masterVolume = PlayerPrefs.GetFloat($"Settings.Volume.0", 0.5f);
			SetSoundVolume(Audio.AudioType.Master, masterVolume);
			var musicVolume = PlayerPrefs.GetFloat($"Settings.Volume.1", 0.5f);
			SetSoundVolume(Audio.AudioType.BGM, musicVolume);
			var sfxVolume = PlayerPrefs.GetFloat($"Settings.Volume.2", 0.5f);
			SetSoundVolume(Audio.AudioType.SFX, sfxVolume);
			var uiVolume = PlayerPrefs.GetFloat($"Settings.Volume.3", 1f);
			SetSoundVolume(Audio.AudioType.UI, uiVolume);
			var voiceVolume = PlayerPrefs.GetFloat($"Settings.Volume.4", 1f);
			SetSoundVolume(Audio.AudioType.Voice, voiceVolume);

			OnRestoreComplete?.Invoke();
		}

		#endregion
		// ------------------------------------------------------------------------------------------------------------
		#region 기본

		#region 데미지 팝업 여부
		public static event System.Action<bool> DamageVisibleChanged;

		/// <summary>
		/// 데미지 팝업이 화면에 보일지 여부를 결정합니다.
		/// </summary>
		public static bool DamageVisible
		{
			get => System.Convert.ToBoolean(PlayerPrefs.GetInt($"Settings.DamageVisible", 1));
			set
			{
				PlayerPrefs.SetInt("Settings.DamageVisible", System.Convert.ToInt16(value));
				PlayerPrefs.Save();

				DamageVisibleChanged?.Invoke(value);
			}
		}
        #endregion

        #region 최대 FPS
        private static int[] FPSOptions = { 25, 30, 60, 80, 120, 144, 200, 240, -1 };
		public static string[] FPSOptionNames = { "25 FPS", "30 FPS", "60 FPS", "80 FPS", "120 FPS", "144 FPS", "200 FPS", "240 FPS", "NoLlimit" };

		/// <summary>
		/// FPS 옵션의 인덱스를 가져오거나 설정합니다.
		/// </summary>
		public static int MaxFPSIndex
		{
			get => System.Array.IndexOf(FPSOptions, Application.targetFrameRate);
			set
			{
				if (value >= 0 && value < FPSOptions.Length)
				{
					Application.targetFrameRate = FPSOptions[value];
					PlayerPrefs.SetInt("Settings.MaxFPS", value);
					PlayerPrefs.Save();
				}
			}
		}
		#endregion

		#region 그래픽 품질
		/// <summary> 
		/// 정의된 품질 수준 목록의 인덱스를 통해 사용할 품질 수준을 가져오거나 설정합니다. 
		/// 이러한 품질 수준은 품질 설정 편집기에서 생성됩니다.
		/// 메뉴: Edit > Project Settings > Quality
		/// </summary>
		public static int GFXQualityLevelIndex
		{
			get => QualitySettings.GetQualityLevel();
			set
			{
#if !UNITY_EDITOR
				QualitySettings.SetQualityLevel(value);
#endif
				PlayerPrefs.SetInt("Settings.Quality", value);
				PlayerPrefs.Save();
			}
		}
		#endregion

		#endregion
		// ------------------------------------------------------------------------------------------------------------
		#region 소리 설정

		// 소리 유형이 변경될 때 알리기 위한 핸들러
		public static event System.Action<Audio.AudioType, float> SoundVolumeChanged;

		/// <summary>
		/// 지정된 소리 유형의 볼륨을 설정합니다.
		/// 범위는 0(소리 없음)과 1(최대) 사이의 부동 소수점 값입니다.
		/// </summary>
		public static void SetSoundVolume(Audio.AudioType type, float value)
		{
			int idx = (int)type;
			value = Mathf.Clamp01(value);
			PlayerPrefs.SetFloat($"Settings.Volume.{idx}", value);
			PlayerPrefs.Save();

			switch (type)
			{
				case Audio.AudioType.Master:
					SoundManager.GlobalVolume = value;
					break;
				case Audio.AudioType.BGM:
					SoundManager.GlobalBGMVolume = value;
					break;
				case Audio.AudioType.SFX:
					SoundManager.GlobalSFXVolume = value;
					break;
				case Audio.AudioType.UI:
					SoundManager.GlobalUIVolume = value;
					break;
				case Audio.AudioType.Voice:
					SoundManager.GlobalVoiceVolume = value;
					break;
			}

			SoundVolumeChanged?.Invoke(type, value);
		}

		/// <summary>
		/// 지정된 소리 유형의 볼륨을 가져옵니다.
		/// 값은 0(소리 없음)과 1(최대) 사이의 부동 소수점 값입니다.
		/// </summary>
		public static float GetSoundVolume(Audio.AudioType type)
		{
			switch (type)
			{
				case Audio.AudioType.Master:
					return SoundManager.GlobalVolume;
				case Audio.AudioType.BGM:
					return SoundManager.GlobalBGMVolume;
				case Audio.AudioType.SFX:
					return SoundManager.GlobalSFXVolume;
				case Audio.AudioType.UI:
					return SoundManager.GlobalUIVolume;
				case Audio.AudioType.Voice:
					return SoundManager.GlobalVoiceVolume;
			}
			return 0f;
		}

		#endregion
		// ------------------------------------------------------------------------------------------------------------

	}
}