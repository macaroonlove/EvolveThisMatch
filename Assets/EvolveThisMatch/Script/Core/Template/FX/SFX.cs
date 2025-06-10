using FrameWork.Editor;
using FrameWork.Sound;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/SFX", fileName = "SFX_", order = 3)]
    public class SFX : FX
    {
        [SerializeField, Label("오디오 소스")] private AudioClip _clip;

        public override void Play(Unit target)
        {
            if (_clip == null) return;

            SoundManager.PlaySFX(_clip);
        }

        public override void Play(Vector3 pos)
        {
            if (_clip == null) return;

            SoundManager.PlaySFX(_clip);
        }
    }
}