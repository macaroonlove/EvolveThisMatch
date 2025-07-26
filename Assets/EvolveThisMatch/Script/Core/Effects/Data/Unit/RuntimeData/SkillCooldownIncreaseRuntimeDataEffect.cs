using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Effect/SkillCooldownIncreaseRuntimeDataEffect", fileName = "SkillCooldownIncreaseRuntimeDataEffect", order = 0)]
    public class SkillCooldownIncreaseRuntimeDataEffect : SkillCooldownIncreaseDataEffect, IRuntimeDataEffect
    {
        public string GetTitle() => "스킬 가속 증가(%)";

        public void SetValue(float value)
        {
            _value = Mathf.Round(value * 0.01f * 10000f) / 10000f;
        }
    }
}