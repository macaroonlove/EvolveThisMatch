using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Poison/Release Poison", fileName = "Release Poison", order = 1)]
    public class ReleasePoisonFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderKeyword("_ENABLEPOISON_ON", false);
        }
    }
}