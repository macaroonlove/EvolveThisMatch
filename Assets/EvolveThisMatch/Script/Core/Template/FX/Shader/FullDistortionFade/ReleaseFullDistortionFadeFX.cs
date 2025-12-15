using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/FullDistortionFade/Release FullDistortionFade", fileName = "Release FullDistortionFade", order = 1)]
    public class ReleaseFullDistortionFadeFX : ShaderFX
    {
        [SerializeField] private int _startDelay;
        [SerializeField] private float _fadeTime;
        [SerializeField] private int _offDelay;

        public override async void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();

            await UniTask.Delay(_startDelay);
            fxAbility.FadeIn("_FullDistortionFade", _fadeTime);
            await UniTask.Delay(_offDelay);
            fxAbility.SetShaderKeyword("_ENABLEFULLDISTORTION_ON", false);
        }
    }
}