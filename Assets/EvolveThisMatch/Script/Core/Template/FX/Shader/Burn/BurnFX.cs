using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Burn/Burn", fileName = "Burn", order = 0)]
    public class BurnFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderKeyword("_ENABLEBURN_ON", true);
            fxAbility.Fade("_BurnFade", 0.5f, 0.0f, 4.0f);
        }
    }
}