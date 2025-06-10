using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Frozen/Frozen", fileName = "Frozen", order = 0)]
    public class FrozenFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderKeyword("_ENABLEFROZEN_ON", true);
            fxAbility.FadeIn("_FrozenFade", 0.5f);
        }
    }
}