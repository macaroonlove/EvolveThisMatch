using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Spawn/Spawn", fileName = "Spawn", order = 0)]
    public class SpawnFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.Fade("_FullGlowDissolveFade", 0.5f, 0.5f, 1f);
        }
    }
}