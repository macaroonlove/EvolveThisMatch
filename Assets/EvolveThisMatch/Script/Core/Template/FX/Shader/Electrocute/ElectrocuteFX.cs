using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Electrocute/Electrocute", fileName = "Electrocute", order = 0)]
    public class ElectrocuteFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderKeyword("_ENABLETEXTURELAYER1_ON", true);
        }
    }
}