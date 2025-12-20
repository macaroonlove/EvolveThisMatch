using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class SkillCooldownIncreaseDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "스킬 가속을 증가·감소 시켜주세요.";
            }
            else if (_value > 0)
            {
                return $"스킬 가속  {_value * 100}% 증가";
            }
            else
            {
                return $"스킬 가속  {Mathf.Abs(_value) * 100}% 감소";
            }
        }
    }
}