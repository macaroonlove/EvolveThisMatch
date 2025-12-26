using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Spawn/EnemySpawn", fileName = "EnemySpawn", order = 1)]
    public class EnemySpawnFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.Fade("_HologramFade", 0.5f, 1f, 0f);
        }
    }
}