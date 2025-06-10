using UnityEngine;

namespace FrameWork.Sound
{
    public class Audio
    {
        public enum AudioType
        {
            Master,
            BGM,
            SFX,
            UI,
            Voice
        }

        /// <summary>
        /// 오디오 ID
        /// <para>오디오 객체가 생성될 때 마다 +1씩 증가한다.</para>
        /// </summary>
        public int AudioID { get; private set; }

        /// <summary>
        /// 오디오의 타입
        /// </summary>
        public AudioType Type { get; private set; }

        /// <summary>
        /// 오디오가 재생 중인지 여부
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// 오디오가 일시 정지되었는지 여부
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// 오디오가 완전히 정지되었는지 여부
        /// </summary>
        public bool IsStopped { get; private set; }

        /// <summary>
        /// 오디오가 생성되고 최소한 한 번 업데이트되었는지 여부
        /// </summary>
        public bool IsUpdated { get; private set; }

        /// <summary>
        /// 오디오가 현재 풀링되어 있는지 여부
        /// </summary>
        public bool IsPooled { get; set; }

        /// <summary>
        /// 씬 전환 시, 오디오가 지속될지 여부
        /// </summary>
        public bool Persist { get; set; }

        /// <summary>
        /// 오디오가 페이드 인 되는데 걸리는 시간 (시작할 때)
        /// <para>오디오의 목표 볼륨이 현재 볼륨보다 높은 경우, 목표 볼륨에 도달하는 데 소요되는 시간</para>
        /// </summary>
        public float FadeInSeconds { get; set; }

        /// <summary>
        /// 오디오가 페이드 아웃 되는데 걸리는 시간 (정지할 때)
        /// <para>오디오의 목표 볼륨이 현재 볼륨보다 작은 경우, 목표 볼륨에 도달하는 데 소요되는 시간</para>
        /// </summary>
        public float FadeOutSeconds { get; set; }

        /// <summary>
        /// 오디오 볼륨
        /// <para>SetVolume 메서드를 사용하여 값을 변경할 수 있습니다.</para>
        /// </summary>
        public float Volume { get; private set; }

        /// <summary>
        /// 오디오 소스
        /// </summary>
        public AudioSource AudioSource { get; private set; }

        /// <summary>
        /// AudioSource가 어떤 객체에 추가될지를 결정할 수 있습니다.
        /// <para>※ 따로 설정을 안해줄 경우, SoundManager객체에 부착됩니다.</para>
        /// </summary>
        public Transform SourceTransform
        {
            get { return sourceTransform; }
            set
            {
                if (value == null)
                {
                    sourceTransform = SoundManager.Transform;
                }
                else
                {
                    sourceTransform = value;
                }
            }
        }

        #region AudioSource 필드 설정
        private AudioClip clip;
        private bool loop;
        private bool mute;
        private int priority;
        private float pitch;
        private float stereoPan;
        private float spatialBlend;
        private float reverbZoneMix;
        private float dopplerLevel;
        private float spread;
        private AudioRolloffMode rolloffMode;
        private float max3DDistance;
        private float min3DDistance;
        #endregion

        #region AudioSource 프로퍼티 설정
        /// <summary>
        /// 오디오 클립
        /// <para>※ 클립을 수정하면, 자동으로 AudioSource의 Clip에 적용됩니다.</para>
        /// </summary>
        public AudioClip Clip
        {
            get { return clip; }
            set
            {
                clip = value;

                if (AudioSource != null)
                {
                    AudioSource.clip = clip;
                }
            }
        }

        /// <summary>
        /// 오디오가 반복될지 여부
        /// <para>※ 수정하면, 자동으로 AudioSource의 loop에 적용됩니다.</para>
        /// </summary>
        public bool Loop
        {
            get { return loop; }
            set
            {
                loop = value;

                if (AudioSource != null)
                {
                    AudioSource.loop = loop;
                }
            }
        }

        /// <summary>
        /// 오디오가 음소거되어 있는지 여부
        /// </summary>
        public bool Mute
        {
            get { return mute; }
            set
            {
                mute = value;

                if (AudioSource != null)
                {
                    AudioSource.mute = mute;
                }
            }
        }

        /// <summary>
        /// 오디오의 우선순위를 설정합니다.
        /// </summary>
        /// <value>0 ~ 256</value>
        public int Priority
        {
            get { return priority; }
            set
            {
                priority = Mathf.Clamp(value, 0, 256);

                if (AudioSource != null)
                {
                    AudioSource.priority = priority;
                }
            }
        }

        /// <summary>
        /// 오디오의 피치(소리의 높낮이)를 설정합니다.
        /// <para>피치는 를 결정하며, 기본값은 1.0입니다. 값이 0.5이면 반음이 낮아지고, 2.0이면 두 배 높아집니다.</para>
        /// </summary>
        /// <value>-3.0(낮아짐) ~ 3.0(높아짐)</value>
        public float Pitch
        {
            get { return pitch; }
            set
            {
                pitch = Mathf.Clamp(value, -3.0f, 3.0f);

                if (AudioSource != null)
                {
                    AudioSource.pitch = pitch;
                }
            }
        }

        /// <summary>
        /// 재생 중인 오디오의 좌우 채널(L/R) 벨런스를 설정합니다.
        /// <para>Stereo 채널에만 적용됩니다. (SpatialBlend가 1에 근접할수록)</para>
        /// </summary>
        /// <value>-1.0(왼쪽) ~ 1.0(오른쪽)</value>
        public float StereoPan
        {
            get { return stereoPan; }
            set
            {
                stereoPan = Mathf.Clamp(value, -1.0f, 1.0f);

                if (AudioSource != null)
                {
                    AudioSource.panStereo = stereoPan;
                }
            }
        }

        /// <summary>
        /// 재생 중인 오디오의 Mono와 Stereo 간의 비율을 설정합니다.
        /// </summary>
        /// <value>0.0(2D, Mono) ~ 1.0(3D, Stereo)</value>
        public float SpatialBlend
        {
            get { return spatialBlend; }
            set
            {
                spatialBlend = Mathf.Clamp01(value);

                if (AudioSource != null)
                {
                    AudioSource.spatialBlend = spatialBlend;
                }
            }
        }

        /// <summary>
        /// 오디오가 얼마나 공간에 반사되면서 울릴지(리버브 효과)를 설정할 수 있습니다.
        /// </summary>
        /// <value>
        /// 0.0(리버브 없음) ~ 2.0(강한 리버브)
        /// <para>기본: 1.0</para>
        /// </value>
        public float ReverbZoneMix
        {
            get { return reverbZoneMix; }
            set
            {
                reverbZoneMix = Mathf.Clamp(value, 0, 2.0f);

                if (AudioSource != null)
                {
                    AudioSource.reverbZoneMix = reverbZoneMix;
                }
            }
        }

        #region 3D 세팅
        /// <summary>
        /// 오디오에 도플러 효과를 설정할 수 있습니다.
        /// <para>도플러 효과: 소리가 이동 속도에 따라 피치가 변하는 효과</para>
        /// </summary>
        /// <value>0.0(속도에 따른 피치 변화 없음) ~ 5.0(속도에 따른 피치 변화 극대화)</value>
        public float DopplerLevel
        {
            get { return dopplerLevel; }
            set
            {
                dopplerLevel = Mathf.Clamp(value, 0, 5.0f);

                if (AudioSource != null)
                {
                    AudioSource.dopplerLevel = dopplerLevel;
                }
            }
        }

        /// <summary>
        /// 오디오가 얼마나 넓게 퍼질지 설정할 수 있습니다.
        /// <para>※ Stereo 채널에서만 동작하여 소리의 방향과 입체감을 제공합니다.</para>
        /// </summary>
        /// <value>
        /// <para>0: 소리가 한쪽(좌 or 우)에서만 들림</para>
        /// <para>180: 소리가 좌측과 우측 모두에서 들림</para>
        /// <para>360: 소리가 전방위적으로 퍼져서 주위에서 소리가 나오는 것 처럼 들림</para>
        /// </value>
        public float Spread
        {
            get { return spread; }
            set
            {
                spread = Mathf.Clamp(value, 0, 360.0f);

                if (AudioSource != null)
                {
                    AudioSource.spread = spread;
                }
            }
        }

        /// <summary>
        /// 오디오 소스와 리스너의 거리에 따라 소리의 감소되는 방식을 설정할 수 있습니다.
        /// </summary>
        public AudioRolloffMode RolloffMode
        {
            get { return rolloffMode; }
            set
            {
                rolloffMode = value;

                if (AudioSource != null)
                {
                    AudioSource.rolloffMode = rolloffMode;
                }
            }
        }

        /// <summary>
        /// 소리의 최대 거리, 이 거리를 초과하면 소리가 들리지 않습니다.
        /// </summary>
        public float Max3DDistance
        {
            get { return max3DDistance; }
            set
            {
                max3DDistance = Mathf.Max(value, 0.01f);

                if (AudioSource != null)
                {
                    AudioSource.maxDistance = max3DDistance;
                }
            }
        }

        /// <summary>
        /// 소리가 명확하게 들리는 최대 거리
        /// <para>※ 이 거리를 초과하면 소리의 볼륨은 거리와 함께 감소</para>
        /// </summary>
        public float Min3DDistance
        {
            get { return min3DDistance; }
            set
            {
                min3DDistance = Mathf.Max(value, 0);

                if (AudioSource != null)
                {
                    AudioSource.minDistance = min3DDistance;
                }
            }
        }
        #endregion
        #endregion

        private static int audioCounter = 0;

        private float targetVolume;
        private float initTargetVolume;
        private float fadeInterpolater;
        private float onFadeStartVolume;
        private Transform sourceTransform;

        public Audio(AudioType audioType, AudioClip clip, bool loop, bool persist, float volume, float fadeInValue, float fadeOutValue, Transform sourceTransform)
        {
            AudioID = audioCounter;
            audioCounter++;

            this.Type = audioType;
            this.Clip = clip;
            this.SourceTransform = sourceTransform;
            this.Loop = loop;
            this.Persist = persist;
            this.targetVolume = volume;
            this.initTargetVolume = volume;
            this.FadeInSeconds = fadeInValue;
            this.FadeOutSeconds = fadeOutValue;

            Mute = false;
            Volume = 0f;
            Priority = 128;
            Pitch = 1;
            StereoPan = 0;
            if (sourceTransform != null && sourceTransform != SoundManager.Transform)
            {
                SpatialBlend = 1;
            }
            ReverbZoneMix = 1;

            DopplerLevel = 1;
            Spread = 0;
            RolloffMode = AudioRolloffMode.Logarithmic;
            Min3DDistance = 1;
            Max3DDistance = 500;

            IsPlaying = false;
            IsPaused = false;
            IsUpdated = false;
            IsPooled = false;
        }

        #region 오디오 재생
        /// <summary>
        /// 오디오 클립을 처음부터 재생합니다.
        /// </summary>
        public void Play()
        {
            Play(initTargetVolume);
        }

        /// <summary>
        /// 오디오 클립을 처음부터 재생합니다.
        /// </summary>
        /// <param name="volume">해당 오디오의 볼륨을 따로 설정할 수 있습니다.</param>
        public void Play(float volume)
        {
            if (IsPooled)
            {
                bool restoredFromPool = SoundManager.RestoreAudioFromPool(Type, AudioID);

                if (!restoredFromPool) return;

                IsPooled = false;
            }

            if (AudioSource == null)
            {
                CreateAudiosource();
            }

            AudioSource.Play();
            IsPlaying = true;

            fadeInterpolater = 0f;
            onFadeStartVolume = this.Volume;
            targetVolume = volume;
        }

        private void CreateAudiosource()
        {
            AudioSource = SourceTransform.gameObject.AddComponent<AudioSource>();
            AudioSource.clip = Clip;
            AudioSource.loop = Loop;
            AudioSource.mute = Mute;
            AudioSource.volume = Volume;
            AudioSource.priority = Priority;
            AudioSource.pitch = Pitch;
            AudioSource.panStereo = StereoPan;
            AudioSource.spatialBlend = SpatialBlend;
            AudioSource.reverbZoneMix = ReverbZoneMix;
            AudioSource.dopplerLevel = DopplerLevel;
            AudioSource.spread = Spread;
            AudioSource.rolloffMode = RolloffMode;
            AudioSource.maxDistance = Max3DDistance;
            AudioSource.minDistance = Min3DDistance;
        }
        #endregion

        #region 오디오 정지/일시 정지/다시 시작
        public void Stop()
        {
            fadeInterpolater = 0f;
            onFadeStartVolume = Volume;
            targetVolume = 0f;

            IsStopped = true;
        }

        public void Pause()
        {
            AudioSource.Pause();
            IsPaused = true;
        }

        public void Resume()
        {
            AudioSource.UnPause();
            IsPaused = false;
        }
        #endregion

        public void Update()
        {
            if (AudioSource == null) return;

            IsUpdated = true;

            // 현재 볼륨이 목표 볼륨과 다르다면, 현재 볼륨을 목표 볼륨까지 높이거나 낮춥니다.
            if (Volume != targetVolume)
            {
                float fadeValue;
                fadeInterpolater += Time.unscaledDeltaTime;

                if (Volume > targetVolume) fadeValue = FadeOutSeconds;
                else fadeValue = FadeInSeconds;

                Volume = Mathf.Lerp(onFadeStartVolume, targetVolume, fadeInterpolater / fadeValue);
            }

            // 볼륨을 조절합니다.
            switch (Type)
            {
                case AudioType.BGM:
                    {
                        AudioSource.volume = Volume * SoundManager.GlobalBGMVolume * SoundManager.GlobalVolume;
                        break;
                    }
                case AudioType.SFX:
                    {
                        AudioSource.volume = Volume * SoundManager.GlobalSFXVolume * SoundManager.GlobalVolume;
                        break;
                    }
                case AudioType.UI:
                    {
                        AudioSource.volume = Volume * SoundManager.GlobalUIVolume * SoundManager.GlobalVolume;
                        break;
                    }
                case AudioType.Voice:
                    {
                        AudioSource.volume = Volume * SoundManager.GlobalVoiceVolume * SoundManager.GlobalVolume;
                        break;
                    }
            }

            // 오디오를 완전히 정지시킵니다.
            if (Volume == 0f && IsStopped)
            {
                AudioSource.Stop();
                IsPlaying = false;
                IsStopped = false;
                IsPaused = false;
            }

            // 어플리케이션이 활성화 되었을 때, 오디오가 재생되고 있는지 확인합니다.
            if (AudioSource.isPlaying != IsPlaying && Application.isFocused)
            {
                IsPlaying = AudioSource.isPlaying;
            }
        }
    }
}