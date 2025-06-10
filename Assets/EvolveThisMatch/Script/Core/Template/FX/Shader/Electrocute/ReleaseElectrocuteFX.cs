using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Electrocute/Release Electrocute", fileName = "Release Electrocute", order = 1)]
    public class ReleaseElectrocuteFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderKeyword("_ENABLETEXTURELAYER1_ON", false);
        }
    }
}