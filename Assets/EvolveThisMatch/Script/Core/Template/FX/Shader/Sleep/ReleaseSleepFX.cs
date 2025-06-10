using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Sleep/Release Sleep", fileName = "Release Sleep", order = 1)]
    public class ReleaseSleepFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderKeyword("_ENABLEENCHANTED_ON", false);
        }
    }
}