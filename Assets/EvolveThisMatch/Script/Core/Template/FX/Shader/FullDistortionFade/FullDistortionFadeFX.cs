using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/FullDistortionFade/FullDistortionFade", fileName = "FullDistortionFade", order = 0)]
    public class FullDistortionFadeFX : ShaderFX
    {
        public override async void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();

            await UniTask.Delay(100);
            fxAbility.FadeOut("_FullDistortionFade", 0.5f);
        }
    }
}