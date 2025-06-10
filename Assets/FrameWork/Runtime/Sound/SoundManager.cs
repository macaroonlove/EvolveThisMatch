using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameWork.Sound
{
    /// <summary>
    /// 게임에 사용되는 모든 소리를 관리하는 매니저
    /// </summary>
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        /// <summary>
        /// SoundManager가 연결된 GameObject의 Transform
        /// </summary>
        public static Transform Transform { get { return Instance.transform; } }

        #region 중복된 오디오 클립 무시 여부
        public static bool IgnoreDuplicateBGM { get; set; }
        public static bool IgnoreDuplicateSFX { get; set; }
        public static bool IgnoreDuplicateUISound { get; set; }
        public static bool IgnoreDuplicateVoice { get; set; }
        #endregion

        #region 음량 조절
        public static float GlobalVolume { get; set; }
        public static float GlobalBGMVolume { get; set; }
        public static float GlobalSFXVolume { get; set; }
        public static float GlobalUIVolume { get; set; }
        public static float GlobalVoiceVolume { get; set; }
        #endregion

        #region 오디오 클립을 가지고 있을 Dictionary
        private static Dictionary<int, Audio> bgmAudio;
        private static Dictionary<int, Audio> sfxAudio;
        private static Dictionary<int, Audio> uiSoundAudio;
        private static Dictionary<int, Audio> voiceAudio;
        private static Dictionary<int, Audio> audioPool;
        #endregion

        private static bool initialized = false;

        protected override void Initialize()
        {
            if (!initialized)
            {
                bgmAudio = new Dictionary<int, Audio>();
                sfxAudio = new Dictionary<int, Audio>();
                uiSoundAudio = new Dictionary<int, Audio>();
                voiceAudio = new Dictionary<int, Audio>();
                audioPool = new Dictionary<int, Audio>();

                GlobalVolume = 0.5f;
                GlobalBGMVolume = 0.5f;
                GlobalSFXVolume = 0.5f;
                GlobalUIVolume = 0.5f;
                GlobalVoiceVolume = 0.5f;

                IgnoreDuplicateBGM = false;
                IgnoreDuplicateSFX = false;
                IgnoreDuplicateUISound = false;
                IgnoreDuplicateVoice = false;

                initialized = true;
            }
        }

        #region 씬 로드 시 초기화 로직
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// 모든 오디오를 중지하고 제거합니다.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RemoveNonPersistAudio(bgmAudio);
            RemoveNonPersistAudio(sfxAudio);
            RemoveNonPersistAudio(uiSoundAudio);
            RemoveNonPersistAudio(voiceAudio);
        }

        /// <summary>
        /// 씬이 전환될 때, 지속되지 않을 오디오를 Audio Dictionary에서 제거
        /// </summary>
        private static void RemoveNonPersistAudio(Dictionary<int, Audio> audioDict)
        {
            List<int> keys = new List<int>(audioDict.Keys);

            foreach (int key in keys)
            {
                Audio audio = audioDict[key];

                if (!audio.Persist && audio.IsUpdated)
                {
                    Destroy(audio.AudioSource);
                    audioDict.Remove(key);
                }
            }

            keys = new List<int>(audioPool.Keys);

            foreach (int key in keys)
            {
                Audio audio = audioPool[key];

                if (!audio.Persist && audio.IsUpdated)
                {
                    audioPool.Remove(key);
                }
            }
        }
        #endregion

        #region 오디오 업데이트
        private void Update()
        {
            UpdateAllAudio(bgmAudio);
            UpdateAllAudio(sfxAudio);
            UpdateAllAudio(uiSoundAudio);
            UpdateAllAudio(voiceAudio);
        }

        /// <summary>
        /// Audio Dictionary의 모든 오디오 상태를 업데이트합니다.
        /// </summary>
        private static void UpdateAllAudio(Dictionary<int, Audio> audioDict)
        {
            List<int> keys = new List<int>(audioDict.Keys);

            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                audio.Update();

                // 오디오가 완전히 정지되었다면
                if (!audio.IsPlaying && !audio.IsPaused)
                {
                    Destroy(audio.AudioSource);

                    audioPool.Add(key, audio);
                    audio.IsPooled = true;
                    audioDict.Remove(key);
                }
            }
        }
        #endregion

        #region 유틸리티
        /// <summary>
        /// AudioType에 따라 오디오 사전을 반환합니다.
        /// </summary>
        private static Dictionary<int, Audio> GetAudioTypeDictionary(Audio.AudioType audioType)
        {
            Dictionary<int, Audio> audioDict = new Dictionary<int, Audio>();

            switch (audioType)
            {
                case Audio.AudioType.BGM:
                    audioDict = bgmAudio;
                    break;
                case Audio.AudioType.SFX:
                    audioDict = sfxAudio;
                    break;
                case Audio.AudioType.UI:
                    audioDict = uiSoundAudio;
                    break;
                case Audio.AudioType.Voice:
                    audioDict = voiceAudio;
                    break;
            }

            return audioDict;
        }

        /// <summary>
        /// 풀링된 오디오를 복원하여 해당 Audio Dictionary에 다시 추가합니다.
        /// </summary>
        public static bool RestoreAudioFromPool(Audio.AudioType audioType, int audioID)
        {
            if (audioPool.ContainsKey(audioID))
            {
                Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);
                audioDict.Add(audioID, audioPool[audioID]);
                audioPool.Remove(audioID);

                return true;
            }

            return false;
        }
        #endregion

        #region GetAudio (오디오 객체를 받아오는 메서드)
        public static Audio GetBGMAudio(int audioID)
        {
            return GetAudio(Audio.AudioType.BGM, true, audioID);
        }

        public static Audio GetBGMAudio(AudioClip audioClip)
        {
            return GetAudio(Audio.AudioType.BGM, true, audioClip);
        }

        public static Audio GetSFXAudio(int audioID)
        {
            return GetAudio(Audio.AudioType.SFX, true, audioID);
        }

        public static Audio GetSFXAudio(AudioClip audioClip)
        {
            return GetAudio(Audio.AudioType.SFX, true, audioClip);
        }

        public static Audio GetUISoundAudio(int audioID)
        {
            return GetAudio(Audio.AudioType.UI, true, audioID);
        }

        public static Audio GetUISoundAudio(AudioClip audioClip)
        {
            return GetAudio(Audio.AudioType.UI, true, audioClip);
        }

        public static Audio GetVoiceAudio(int audioID)
        {
            return GetAudio(Audio.AudioType.Voice, true, audioID);
        }

        public static Audio GetVoiceAudio(AudioClip audioClip)
        {
            return GetAudio(Audio.AudioType.Voice, true, audioClip);
        }

        private static Audio GetAudio(Audio.AudioType audioType, bool usePool, int audioID)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            if (audioDict.ContainsKey(audioID))
            {
                return audioDict[audioID];
            }

            if (usePool && audioPool.ContainsKey(audioID) && audioPool[audioID].Type == audioType)
            {
                return audioPool[audioID];
            }

            return null;
        }

        private static Audio GetAudio(Audio.AudioType audioType, bool usePool, AudioClip audioClip)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<Audio> audioList = new List<Audio>(audioDict.Values);
            foreach (var audio in audioList)
            {
                if (audio.Clip == audioClip && audio.Type == audioType)
                {
                    if (audioDict.ContainsKey(audio.AudioID))
                    {
                        return audio;
                    }
                }
            }

            if (usePool)
            {
                List<Audio> poolList = new List<Audio>(audioPool.Values);
                foreach (var audio in poolList)
                {
                    if (audio.Clip == audioClip && audio.Type == audioType)
                    {
                        if (audioPool.ContainsKey(audio.AudioID))
                        {
                            return audio;
                        }
                    }
                }
            }

            return null;
        }
        #endregion

        #region CreateAudio (오디오 객체를 생성하는 메서드)
        private static int CreateAudio(Audio.AudioType audioType, AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, Transform sourceTransform)
        {
            if (clip == null)
            {
                Debug.Log("<color=#ff0000>[SoundManager] Audio clip이 없습니다.</color>", clip);
            }

            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);
            bool ignoreDuplicateAudio = GetAudioTypeIgnoreDuplicateSetting(audioType);

            if (ignoreDuplicateAudio)
            {
                // 중복된 오디오가 있는지 탐색
                Audio duplicateAudio = GetAudio(audioType, true, clip);

                if (duplicateAudio != null)
                {
                    return duplicateAudio.AudioID;
                }
            }

            Audio audio = new Audio(audioType, clip, loop, persist, volume, fadeInSeconds, fadeOutSeconds, sourceTransform);

            audioDict.Add(audio.AudioID, audio);

            return audio.AudioID;
        }

        /// <summary>
        /// 지정된 오디오 유형의 오디오에 대한 중복 무시 설정을 검색합니다.
        /// </summary>
        private static bool GetAudioTypeIgnoreDuplicateSetting(Audio.AudioType audioType)
        {
            switch (audioType)
            {
                case Audio.AudioType.BGM:
                    return IgnoreDuplicateBGM;
                case Audio.AudioType.SFX:
                    return IgnoreDuplicateSFX;
                case Audio.AudioType.UI:
                    return IgnoreDuplicateUISound;
                case Audio.AudioType.Voice:
                    return IgnoreDuplicateVoice;
                default:
                    return false;
            }
        }
        #endregion

        #region PlayAudio (오디오를 실행시키는 메서드)
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">음악이 반복 재생되는지 여부</param>
        /// <param name="persist">오디오가 씬 전환 간에 유지되는지 여부</param>
        /// <param name="fadeInSeconds">오디오가 페이드 인하여 목표 볼륨에 도달하는 데 필요한 시간(현재 볼륨보다 높을 경우)</param>
        /// <param name="fadeOutSeconds">오디오가 페이드 아웃하여 목표 볼륨에 도달하는 데 필요한 시간(현재 볼륨보다 낮을 경우)</param>
        /// <param name="currentMusicfadeOutSeconds">현재 음악 오디오가 페이드 아웃하는 데 필요한 시간입니다. 이 값이 설정되면 자체 페이드 아웃 시간을 덮어씁니다. -1이 전달되면 현재 음악은 자신의 페이드 아웃 시간을 유지합니다.</param>
        /// <param name="sourceTransform">음악의 소스가 되는 변환(3D 오디오가 됩니다). 3D 오디오가 필요 없으면 null을 사용하세요.</param>
        public static int PlayBGM(AudioClip clip, float volume = 1f, bool loop = false, bool persist = false, float fadeInSeconds = 1f, float fadeOutSeconds = 1f, float currentMusicfadeOutSeconds = -1f, Transform sourceTransform = null)
        {
            return PlayAudio(Audio.AudioType.BGM, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">음악이 반복 재생되는지 여부</param>
        /// <param name="persist">오디오가 씬 전환 간에 유지되는지 여부</param>
        /// <param name="fadeInSeconds">오디오가 페이드 인하여 목표 볼륨에 도달하는 데 필요한 시간(현재 볼륨보다 높을 경우)</param>
        /// <param name="fadeOutSeconds">오디오가 페이드 아웃하여 목표 볼륨에 도달하는 데 필요한 시간(현재 볼륨보다 낮을 경우)</param>
        /// <param name="currentMusicfadeOutSeconds">현재 음악 오디오가 페이드 아웃하는 데 필요한 시간입니다. 이 값이 설정되면 자체 페이드 아웃 시간을 덮어씁니다. -1이 전달되면 현재 음악은 자신의 페이드 아웃 시간을 유지합니다.</param>
        /// <param name="sourceTransform">음악의 소스가 되는 변환(3D 오디오가 됩니다). 3D 오디오가 필요 없으면 null을 사용하세요.</param>
        public static int PlaySFX(AudioClip clip, float volume = 1f, bool loop = false, bool persist = false, float fadeInSeconds = 1f, float fadeOutSeconds = 1f, float currentMusicfadeOutSeconds = -1f, Transform sourceTransform = null)
        {
            return PlayAudio(Audio.AudioType.SFX, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">음악이 반복 재생되는지 여부</param>
        /// <param name="persist">오디오가 씬 전환 간에 유지되는지 여부</param>
        /// <param name="fadeInSeconds">오디오가 페이드 인하여 목표 볼륨에 도달하는 데 필요한 시간(현재 볼륨보다 높을 경우)</param>
        /// <param name="fadeOutSeconds">오디오가 페이드 아웃하여 목표 볼륨에 도달하는 데 필요한 시간(현재 볼륨보다 낮을 경우)</param>
        /// <param name="currentMusicfadeOutSeconds">현재 음악 오디오가 페이드 아웃하는 데 필요한 시간입니다. 이 값이 설정되면 자체 페이드 아웃 시간을 덮어씁니다. -1이 전달되면 현재 음악은 자신의 페이드 아웃 시간을 유지합니다.</param>
        /// <param name="sourceTransform">음악의 소스가 되는 변환(3D 오디오가 됩니다). 3D 오디오가 필요 없으면 null을 사용하세요.</param>
        public static int PlayUISound(AudioClip clip, float volume = 1f, bool loop = false, bool persist = false, float fadeInSeconds = 1f, float fadeOutSeconds = 1f, float currentMusicfadeOutSeconds = -1f, Transform sourceTransform = null)
        {
            return PlayAudio(Audio.AudioType.UI, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">음악이 반복 재생되는지 여부</param>
        /// <param name="persist">오디오가 씬 전환 간에 유지되는지 여부</param>
        /// <param name="fadeInSeconds">오디오가 페이드 인하여 목표 볼륨에 도달하는 데 필요한 시간(현재 볼륨보다 높을 경우)</param>
        /// <param name="fadeOutSeconds">오디오가 페이드 아웃하여 목표 볼륨에 도달하는 데 필요한 시간(현재 볼륨보다 낮을 경우)</param>
        /// <param name="currentMusicfadeOutSeconds">현재 음악 오디오가 페이드 아웃하는 데 필요한 시간입니다. 이 값이 설정되면 자체 페이드 아웃 시간을 덮어씁니다. -1이 전달되면 현재 음악은 자신의 페이드 아웃 시간을 유지합니다.</param>
        /// <param name="sourceTransform">음악의 소스가 되는 변환(3D 오디오가 됩니다). 3D 오디오가 필요 없으면 null을 사용하세요.</param>
        public static int PlayVoice(AudioClip clip, float volume = 1f, bool loop = false, bool persist = false, float fadeInSeconds = 1f, float fadeOutSeconds = 1f, float currentMusicfadeOutSeconds = -1f, Transform sourceTransform = null)
        {
            return PlayAudio(Audio.AudioType.Voice, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        private static int PlayAudio(Audio.AudioType audioType, AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            int audioID = CreateAudio(audioType, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, sourceTransform);

            if (audioType == Audio.AudioType.BGM)
            {
                StopAllBGM(currentMusicfadeOutSeconds);
            }

            GetAudio(audioType, false, audioID).Play();

            return audioID;
        }
        #endregion

        #region StopAudio (특정 오디오를 완전히 정지시키는 메서드)
        public static void StopSFX(int audioID, float fadeOutSeconds = -1)
        {
            StopAudio(Audio.AudioType.SFX, audioID, fadeOutSeconds);
        }

        public static void StopSFX(AudioClip clip, float fadeOutSeconds = -1)
        {
            StopAudio(Audio.AudioType.SFX, clip, fadeOutSeconds);
        }

        public static void StopUISound(AudioClip clip, float fadeOutSeconds = -1)
        {
            StopAudio(Audio.AudioType.UI, clip, fadeOutSeconds);
        }

        public static void StopUISound(int audioID, float fadeOutSeconds = -1)
        {
            StopAudio(Audio.AudioType.UI, audioID, fadeOutSeconds);
        }

        public static void StopVoice(AudioClip clip, float fadeOutSeconds = -1)
        {
            StopAudio(Audio.AudioType.Voice, clip, fadeOutSeconds);
        }

        public static void StopVoice(int audioID, float fadeOutSeconds = -1)
        {
            StopAudio(Audio.AudioType.Voice, audioID, fadeOutSeconds);
        }

        private static void StopAudio(Audio.AudioType audioType, int audioID, float fadeOutSeconds)
        {
            Audio audio = GetAudio(audioType, true, audioID);

            if (audio == null) return;

            if (fadeOutSeconds > 0)
            {
                audio.FadeOutSeconds = fadeOutSeconds;
            }
            audio.Stop();
        }

        private static void StopAudio(Audio.AudioType audioType, AudioClip clip, float fadeOutSeconds)
        {
            Audio audio = GetAudio(audioType, true, clip);

            if (audio == null) return;

            if (fadeOutSeconds > 0)
            {
                audio.FadeOutSeconds = fadeOutSeconds;
            }
            audio.Stop();
        }
        #endregion

        #region StopAllAudio (모든 오디오를 완전히 정지시키는 메서드)
        public static void StopAll(float musicFadeOutSeconds = -1)
        {
            StopAllBGM(musicFadeOutSeconds);
            StopAllSFX(musicFadeOutSeconds);
            StopAllUISound(musicFadeOutSeconds);
            StopAllVoice(musicFadeOutSeconds);
        }

        public static void StopAllBGM(float fadeOutSeconds = -1)
        {
            StopAllAudio(Audio.AudioType.BGM, fadeOutSeconds);
        }

        public static void StopAllSFX(float fadeOutSeconds = -1)
        {
            StopAllAudio(Audio.AudioType.SFX, fadeOutSeconds);
        }

        public static void StopAllUISound(float fadeOutSeconds = -1)
        {
            StopAllAudio(Audio.AudioType.UI, fadeOutSeconds);
        }

        public static void StopAllVoice(float fadeOutSeconds = -1)
        {
            StopAllAudio(Audio.AudioType.Voice, fadeOutSeconds);
        }

        private static void StopAllAudio(Audio.AudioType audioType, float fadeOutSeconds)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                if (fadeOutSeconds > 0)
                {
                    audio.FadeOutSeconds = fadeOutSeconds;
                }
                audio.Stop();
            }
        }
        #endregion

        #region PauseAudio (특정 오디오를 일시 정지시키는 메서드)
        public static void PauseSFX(int audioID)
        {
            PauseAudio(Audio.AudioType.SFX, audioID);
        }

        public static void PauseSFX(AudioClip clip)
        {
            PauseAudio(Audio.AudioType.SFX, clip);
        }

        public static void PauseUISound(AudioClip clip)
        {
            PauseAudio(Audio.AudioType.UI, clip);
        }

        public static void PauseUISound(int audioID)
        {
            PauseAudio(Audio.AudioType.UI, audioID);
        }

        public static void PauseVoice(AudioClip clip)
        {
            PauseAudio(Audio.AudioType.Voice, clip);
        }

        public static void PauseVoice(int audioID)
        {
            PauseAudio(Audio.AudioType.Voice, audioID);
        }

        private static void PauseAudio(Audio.AudioType audioType, int audioID)
        {
            Audio audio = GetAudio(audioType, true, audioID);

            if (audio == null) return;

            audio.Pause();
        }

        private static void PauseAudio(Audio.AudioType audioType, AudioClip clip)
        {
            Audio audio = GetAudio(audioType, true, clip);

            if (audio == null) return;

            audio.Pause();
        }
        #endregion

        #region PauseAllAudio (모든 오디오를 일시 정지시키는 메서드)
        public static void PauseAll()
        {
            PauseAllBGM();
            PauseAllSFX();
            PauseAllUISound();
            PauseAllVoice();
        }

        public static void PauseAllBGM()
        {
            PauseAllAudio(Audio.AudioType.BGM);
        }

        public static void PauseAllSFX()
        {
            PauseAllAudio(Audio.AudioType.SFX);
        }

        public static void PauseAllUISound()
        {
            PauseAllAudio(Audio.AudioType.UI);
        }

        public static void PauseAllVoice()
        {
            PauseAllAudio(Audio.AudioType.Voice);
        }

        private static void PauseAllAudio(Audio.AudioType audioType)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                audio.Pause();
            }
        }
        #endregion

        #region ResumeAudio (특정 오디오를 일시 정지시키는 메서드)
        public static void ResumeSFX(int audioID)
        {
            ResumeAudio(Audio.AudioType.SFX, audioID);
        }

        public static void ResumeSFX(AudioClip clip)
        {
            ResumeAudio(Audio.AudioType.SFX, clip);
        }

        public static void ResumeUISound(AudioClip clip)
        {
            ResumeAudio(Audio.AudioType.UI, clip);
        }

        public static void ResumeUISound(int audioID)
        {
            ResumeAudio(Audio.AudioType.UI, audioID);
        }

        public static void ResumeVoice(AudioClip clip)
        {
            ResumeAudio(Audio.AudioType.Voice, clip);
        }

        public static void ResumeVoice(int audioID)
        {
            ResumeAudio(Audio.AudioType.Voice, audioID);
        }

        private static void ResumeAudio(Audio.AudioType audioType, int audioID)
        {
            Audio audio = GetAudio(audioType, true, audioID);

            if (audio == null) return;

            audio.Resume();
        }

        private static void ResumeAudio(Audio.AudioType audioType, AudioClip clip)
        {
            Audio audio = GetAudio(audioType, true, clip);

            if (audio == null) return;

            audio.Resume();
        }
        #endregion

        #region ResumeAllAudio (일시정지된 모든 오디오를 다시 시작시키는 메서드)
        public static void ResumeAll()
        {
            ResumeAllBGM();
            ResumeAllSFX();
            ResumeAllUISound();
            ResumeAllVoice();
        }

        public static void ResumeAllBGM()
        {
            ResumeAllAudio(Audio.AudioType.BGM);
        }

        public static void ResumeAllSFX()
        {
            ResumeAllAudio(Audio.AudioType.SFX);
        }

        public static void ResumeAllUISound()
        {
            ResumeAllAudio(Audio.AudioType.UI);
        }

        public static void ResumeAllVoice()
        {
            ResumeAllAudio(Audio.AudioType.Voice);
        }

        private static void ResumeAllAudio(Audio.AudioType audioType)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                audio.Resume();
            }
        }
        #endregion
    }
}