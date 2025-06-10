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
		private static readonly Vector2 MinScreenSize = new Vector2(1024, 768);

		// ------------------------------------------------------------------------------------------------------------
		#region 설정 복원

		public static event System.Action OnRestoreComplete;

		/// <summary> 저장된 설정을 복원합니다. </summary>
		public static void RestoreSettings()
		{
			// 닉네임 복원
			string displayName = PlayerPrefs.GetString($"Settings.DisplayName", "이름을 정해주세요.");
			DisplayNameText = displayName;

			// 조작 방식 설정 복원
			bool isSprintToggle = GetControlMode(ControlTarget.Sprint);
			SetControlMode(ControlTarget.Sprint, isSprintToggle);
			bool isCrouchToggle = GetControlMode(ControlTarget.Crouch);
			SetControlMode(ControlTarget.Crouch, isCrouchToggle);

			// 최대 FPS 설정 복원
			int maxFPSIndex = PlayerPrefs.GetInt("Settings.MaxFPS", FPSOptions.Length - 1);
			Application.targetFrameRate = FPSOptions[maxFPSIndex];
			
			// 마우스 감도 설정 복원
			float rotateSensitivity = PlayerPrefs.GetFloat("Settings.RotateSensitivity", 1f);
			RotateSensitivity = rotateSensitivity;

			// 시야 거리 설정 복원
			float viewDistance = PlayerPrefs.GetFloat("Settings.ViewDistance", 1000f);
			ViewDistance = viewDistance;

			// 퀄리티 설정 복원
			int q = PlayerPrefs.GetInt("Settings.Quality", QualitySettings.GetQualityLevel());
			QualitySettings.SetQualityLevel(q);

			// 화면 밝기 설정 복원
			float screenBrightness = ScreenBrightness;
			ScreenBrightness = screenBrightness;

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

			// 마이크 감도 복원
			float voiceTreshold = GetMicrophoneThreshold(MicrophoneType.Voice);
			SetMicrophoneThreshold(MicrophoneType.Voice, voiceTreshold);
			float ambientSountTreshold = GetMicrophoneThreshold(MicrophoneType.AmbientSound);
			SetMicrophoneThreshold(MicrophoneType.AmbientSound, ambientSountTreshold);

			OnRestoreComplete?.Invoke();
		}

		#endregion
		// ------------------------------------------------------------------------------------------------------------
		#region 기본

		#region 닉네임
		// 싱글플레이의 경우
		public static event System.Action<string> DisplayNameChanged;
		// 멀티플레이의 경우
		// public static event System.Action DisplayNameChanged;

		/// <summary>
		/// 닉네임을 변경합니다.
		/// </summary>
		public static string DisplayNameText
		{
			get => PlayerPrefs.GetString($"Settings.DisplayName", "이름을 정해주세요.");
			set
			{
				PlayerPrefs.SetString("Settings.DisplayName", value);
				PlayerPrefs.Save();

				// 싱글 플레이의 경우
				DisplayNameChanged?.Invoke(value);

				// PhotonNetwork.LocalPlayer.NickName = value;
				// DisplayNameChanged?.Invoke();
			}
		}

		public static event System.Action<bool> DisplayNameVisibleChanged;

		/// <summary>
		/// 닉네임이 화면에 보일지 여부를 결정합니다.
		/// </summary>
		public static bool DisplayNameVisible
		{
			get => System.Convert.ToBoolean(PlayerPrefs.GetInt($"Settings.DisplayNameVisible", 0));
			set
			{
				PlayerPrefs.SetInt("Settings.DisplayNameVisible", System.Convert.ToInt16(value));
				PlayerPrefs.Save();

				DisplayNameVisibleChanged?.Invoke(value);
			}
		}
		#endregion

		#region 조작 방식
		public enum ControlTarget
		{
			Sprint,
			Crouch,
		}

		// 회전 감도 변경을 알리기 위한 핸들러
		public static event System.Action<ControlTarget, bool> ControlModeChanged;

		/// <summary> 
		/// 조작 방식을 변경합니다.
		/// </summary>
		public static void SetControlMode(ControlTarget type, bool value)
		{
			int idx = (int)type;

			PlayerPrefs.SetInt($"Settings.ControlMode.{idx}", System.Convert.ToInt16(value));
			PlayerPrefs.Save();

			ControlModeChanged?.Invoke(type, value);
		}

		/// <summary> 
		/// 저장되있는 조작 방식을 받아옵니다.
		/// </summary>
		public static bool GetControlMode(ControlTarget type)
		{
			int idx = (int)type;

			bool value = System.Convert.ToBoolean(PlayerPrefs.GetInt($"Settings.ControlMode.{idx}", 0));

			return value;
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

		#region 회전 감도
		// 회전 감도 변경을 알리기 위한 핸들러
		public static event System.Action<float> RotateSensitivityChanged;

		/// <summary> 
		/// 마우스 감도를 가져오거나 설정합니다. 
		/// </summary>
		public static float RotateSensitivity
		{
			get => PlayerPrefs.GetFloat("Settings.RotateSensitivity", 1f);
			set
			{
				PlayerPrefs.SetFloat("Settings.RotateSensitivity", value);
				PlayerPrefs.Save();
				RotateSensitivityChanged?.Invoke(value);
			}
		}
		#endregion

		#region 시야거리
		// 회전 감도 변경을 알리기 위한 핸들러
		public static event System.Action<float> ViewDistanceChanged;

		/// <summary> 
		/// 시야 거리를 가져오거나 설정합니다. 
		/// </summary>
		public static float ViewDistance
		{
			get => PlayerPrefs.GetFloat("Settings.ViewDistance", 1f);
			set
			{
				PlayerPrefs.SetFloat("Settings.ViewDistance", value);
				PlayerPrefs.Save();
				ViewDistanceChanged?.Invoke(value);
			}
		}
        #endregion

        #endregion
        // ------------------------------------------------------------------------------------------------------------
        #region 그래픽

        #region 해상도
        private static List<Resolution> Resolutions = new List<Resolution>();

		// 해상도 변경을 알리기 위한 핸들러
		public static event System.Action ResolutionChanged;

		/// <summary> 
		/// 사용할 수 있는 화면 해상도를 나타내는 문자열 목록. 
		/// </summary>
		public static List<string> ScreenResolutions
		{
			get
			{
				List<string> l = new List<string>(Screen.resolutions.Length);
				Resolutions.Clear();

				if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
				{
					for (int i = 0; i < Screen.resolutions.Length; i++)
					{
						if (Screen.resolutions[i].width >= MinScreenSize.x && Screen.resolutions[i].height >= MinScreenSize.y)
						{
							Resolutions.Add(Screen.resolutions[i]);
							l.Add(Screen.resolutions[i].ToString());
						}
					}
				}
				else
				{
					for (int i = 0; i < Screen.resolutions.Length; i++)
					{
						if (Screen.resolutions[i].width >= MinScreenSize.x && Screen.resolutions[i].height >= MinScreenSize.y)
						{
							var res = $"{Screen.resolutions[i].width} x {Screen.resolutions[i].height}";
							if (!l.Contains(res))
							{
								Resolutions.Add(Screen.resolutions[i]);
								l.Add(res);
							}
						}
					}
				}

				return l;
			}
		}

		/// <summary> 
		/// 현재 화면 해상도 인덱스를 가져오거나 설정합니다. 
		/// 이는 지원되는 해상도 목록에서 해상도를 나타내는 정수 값입니다.
		/// 지원되는 해상도 목록은 ScreenResolutions 속성을 통해 가져옵니다. 해상도 인덱스를 결정할 수 없으면 -1을 반환합니다. 
		/// </summary>
		public static int ScreenResolutionIndex
		{
			get
			{
				int _screenResolutionIdx = -1;

				int w = Screen.width;
				int h = Screen.height;
				if (Screen.fullScreenMode != FullScreenMode.Windowed)
				{
					w = Screen.currentResolution.width;
					h = Screen.currentResolution.height;
				}

				for (int i = Resolutions.Count - 1; i >= 0; i--)
				{
					if (w == Resolutions[i].width && h == Resolutions[i].height)
					{
						_screenResolutionIdx = i;
						if (Screen.fullScreenMode != FullScreenMode.ExclusiveFullScreen)
						{
							break;
						}
					}
				}

				return _screenResolutionIdx;
			}
			set
			{
				if (value >= 0 && value < Resolutions.Count)
				{
					Screen.SetResolution(Resolutions[value].width, Resolutions[value].height, Screen.fullScreenMode);
					PlayerPrefs.Save();

					ResolutionChanged?.Invoke();
				}
			}
		}
		#endregion

		#region 화면 모드
		public static string[] ScreenModes = { "FullScreen", "Windowed", "NoBorder" };

		public static int ScreenModeIndex
		{
			get
			{
				// 전체화면, 창모드, 테두리 없음 모드를 각각의 인덱스로 반환
				return Screen.fullScreenMode switch
				{
					FullScreenMode.ExclusiveFullScreen => 0,
					FullScreenMode.Windowed => 1,
					FullScreenMode.FullScreenWindow => 2,
					_ => 2,
				};
			}
			set
			{
				// 인덱스에 따라 화면 모드 설정
				FullScreenMode mode = value switch
				{
					0 => FullScreenMode.ExclusiveFullScreen,
					1 => FullScreenMode.Windowed,
					2 => FullScreenMode.FullScreenWindow,
					_ => FullScreenMode.FullScreenWindow,
				};
				SetScreenMode(mode);
			}
		}

		private static void SetScreenMode(FullScreenMode mode)
		{
			int w = Screen.width;
			int h = Screen.height;
			if (Screen.fullScreenMode != FullScreenMode.Windowed)
			{
				w = Screen.currentResolution.width;
				h = Screen.currentResolution.height;
			}

			Screen.SetResolution(w, h, mode);
			PlayerPrefs.Save();

			ResolutionChanged?.Invoke();
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

        #region 화면 밝기
        /// <summary> 
		/// 화면 밝기를 가져오거나 설정합니다. 
		/// </summary>
        public static float ScreenBrightness
		{
			get => PlayerPrefs.GetFloat("Settings.ScreenBrightness", 0f);
			set
			{
				VolumeController.Instance?.SetScreenBrightness(value);
				PlayerPrefs.SetFloat("Settings.ScreenBrightness", value);
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

		#region 마이크 임계값 설정
		public enum MicrophoneType
		{
			Voice,
			AmbientSound,
		}

		public static event System.Action<MicrophoneType, float> MicrophoneThresholdChanged;

		/// <summary>
		/// 지정된 형식의 임계값을 설정합니다.
		/// 범위는 0(소리 없음)과 1(최대) 사이의 부동 소수점 값입니다.
		/// </summary>
		public static void SetMicrophoneThreshold(MicrophoneType type, float value)
		{
			int idx = (int)type;
			value = Mathf.Clamp01(value);
			PlayerPrefs.SetFloat($"Settings.MicrophoneThreshold.{idx}", value);
			PlayerPrefs.Save();
			
			MicrophoneThresholdChanged?.Invoke(type, value);
		}

		/// <summary>
		/// 지정된 형식의 임계값을 가져옵니다.
		/// 범위는 0(소리 없음)과 1(최대) 사이의 부동 소수점 값입니다.
		/// </summary>
		public static float GetMicrophoneThreshold(MicrophoneType type)
		{
			int idx = (int)type;

			float value = PlayerPrefs.GetFloat($"Settings.MicrophoneThreshold.{idx}", 0f);
			
			return value;
		}

		#endregion
		// ------------------------------------------------------------------------------------------------------------

	}
}