using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Burn/Burn", fileName = "Burn", order = 0)]
    public class BurnFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderProperty("_BurnFade", 1);
            fxAbility.Fade("_BurnEdgeNoiseFactor", 0.5f, -30, 0);
        }
    }
}