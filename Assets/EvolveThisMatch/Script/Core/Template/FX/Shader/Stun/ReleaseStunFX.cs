using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Stun/Release Stun", fileName = "Release Stun", order = 1)]
    public class ReleaseStunFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderKeyword("_ENABLEWIGGLE_ON", false);
        }
    }
}