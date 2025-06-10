using System.Collections;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Slow/Release Slow", fileName = "Release Slow", order = 1)]
    public class ReleaseSlowFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.FadeOut("_CamouflageFade", 0.5f);
            target.StartCoroutine(CoPlay(fxAbility));
        }

        private IEnumerator CoPlay(FXAbility fxAbility)
        {
            yield return new WaitForSeconds(0.5f);
            fxAbility.SetShaderKeyword("_ENABLECAMOUFLAGE_ON", false);
        }
    }
}