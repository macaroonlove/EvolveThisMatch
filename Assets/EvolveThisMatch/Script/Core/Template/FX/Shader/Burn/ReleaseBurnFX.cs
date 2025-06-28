using System.Collections;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Burn/Release Burn", fileName = "Release Burn", order = 1)]
    public class ReleaseBurnFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.Fade("_BurnFade", 0.5f, 4.0f, 0.0f);
            target.StartCoroutine(CoPlay(fxAbility));
        }

        private IEnumerator CoPlay(FXAbility fxAbility)
        {
            yield return new WaitForSeconds(0.5f);
            fxAbility.SetShaderKeyword("_ENABLEBURN_ON", false);
        }
    }
}