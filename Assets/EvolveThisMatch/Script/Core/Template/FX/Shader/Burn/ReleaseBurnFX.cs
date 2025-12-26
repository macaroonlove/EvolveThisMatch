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
            fxAbility.Fade("_BurnEdgeNoiseFactor", 0.5f, 0, -30);
            target.StartCoroutine(CoPlay(fxAbility));
        }

        private IEnumerator CoPlay(FXAbility fxAbility)
        {
            yield return new WaitForSeconds(0.5f);
            fxAbility.SetShaderProperty("_BurnFade", 0);
        }
    }
}