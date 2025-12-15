using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Spawn/EnemySpawn", fileName = "EnemySpawn", order = 1)]
    public class EnemySpawnFX : ShaderFX
    {
        public override async void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderKeyword("_ENABLEHOLOGRAM_ON", true);
            fxAbility.Fade("_HologramFade", 0.5f, 1f, 0f);

            await UniTask.Delay(500);

            fxAbility.SetShaderKeyword("_ENABLEHOLOGRAM_ON", false);
        }
    }
}