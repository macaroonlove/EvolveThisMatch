using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Spawn", fileName = "Spawn", order = 0)]
    public class SpawnFX : ShaderFX
    {
        public override async void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderKeyword("_ENABLEFULLGLOWDISSOLVE_ON", true);
            fxAbility.Fade("_FullGlowDissolveFade", 0.5f, 0.5f, 1f);

            await UniTask.Delay(500);

            fxAbility.SetShaderKeyword("_ENABLEFULLGLOWDISSOLVE_ON", false);
        }
    }
}