using FrameWork;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameWork.GameSettings
{
    [RequireComponent(typeof(Volume))]
    public class VolumeController : Singleton<VolumeController>
    {
        [SerializeField] private Volume volume;

        private ColorAdjustments colorAdjustments;

        private void Reset()
        {
            volume = GetComponent<Volume>();
        }

        protected override void Awake()
        {
            base.Awake();

            if (volume != null || TryGetComponent(out volume))
            {
                if (!volume.profile.TryGet(out colorAdjustments))
                {
                    volume.profile.Add(typeof(ColorAdjustments));

                    volume.profile.TryGet(out colorAdjustments);
                }

                colorAdjustments.postExposure.overrideState = true;
            }
        }

        public void SetScreenBrightness(float value)
        {
            colorAdjustments.postExposure.value = value;
        }
    }
}
