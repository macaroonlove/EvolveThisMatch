using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/FullDistortionFade/Release FullDistortionFade", fileName = "Release FullDistortionFade", order = 1)]
    public class ReleaseFullDistortionFadeFX : ShaderFX
    {
        public override async void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();

            await UniTask.Delay(500);
            fxAbility.FadeIn("_FullDistortionFade", 0.5f);
            await UniTask.Delay(500);
            fxAbility.SetShaderKeyword("_ENABLEFULLDISTORTION_ON", false);
        }
    }
}