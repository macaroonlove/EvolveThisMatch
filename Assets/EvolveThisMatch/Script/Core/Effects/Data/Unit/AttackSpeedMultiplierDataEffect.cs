using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AttackSpeedMultiplierDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            return FormatDescription(_value);
        }

        public override string GetDescription(int level)
        {
            return FormatDescription(GetValue(level));
        }

        public override float GetValue(int level)
        {
            return _value + 0.01f * (level - 1);
        }

        private string FormatDescription(float value)
        {
            if (value == 0)
            {
                return "공격속도를 상승·하락 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"공격속도  {value * 100}% 상승";
            }
            else
            {
                return $"공격속도  {Mathf.Abs(value) * 100}% 하락";
            }
        }
    }
}