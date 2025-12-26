using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Frozen/Release Frozen", fileName = "Release Frozen", order = 1)]
    public class ReleaseFrozenFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.FadeOut("_FrozenFade", 0.5f);
        }
    }
}