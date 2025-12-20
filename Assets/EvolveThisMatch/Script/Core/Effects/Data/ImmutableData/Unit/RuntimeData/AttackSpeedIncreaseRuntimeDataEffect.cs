using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Effect/AttackSpeedIncreaseRuntimeDataEffect", fileName = "AttackSpeedIncreaseRuntimeDataEffect", order = 0)]
    public class AttackSpeedIncreaseRuntimeDataEffect : AttackSpeedIncreaseDataEffect, IRuntimeDataEffect
    {
        public string GetTitle() => "공격간격 감소(%)";

        public void SetValue(float value)
        {
            _value = Mathf.Round(value * 0.01f * 10000f) / 10000f;
        }
    }
}