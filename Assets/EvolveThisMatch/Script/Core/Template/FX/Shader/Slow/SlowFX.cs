using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Slow/Slow", fileName = "Slow", order = 0)]
    public class SlowFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderKeyword("_ENABLECAMOUFLAGE_ON", true);
            fxAbility.FadeIn("_CamouflageFade", 0.5f);
        }
    }
}