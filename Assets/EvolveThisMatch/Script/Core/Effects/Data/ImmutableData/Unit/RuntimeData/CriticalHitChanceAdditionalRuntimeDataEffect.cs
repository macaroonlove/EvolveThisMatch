using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Effect/CriticalHitChanceAdditionalRuntimeDataEffect", fileName = "CriticalHitChanceAdditionalRuntimeDataEffect", order = 0)]
    public class CriticalHitChanceAdditionalRuntimeDataEffect : CriticalHitChanceAdditionalDataEffect, IRuntimeDataEffect
    {
        public string GetTitle() => "치명타 확률 추가";

        public void SetValue(float value)
        {
            _value = (int)value;
        }
    }
}