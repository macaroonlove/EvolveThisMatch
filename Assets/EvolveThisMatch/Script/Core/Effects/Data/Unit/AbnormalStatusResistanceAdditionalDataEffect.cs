using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AbnormalStatusResistanceAdditionalDataEffect : DataEffect<float>
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
                return "상태이상 저항력을 추가하거나 줄여주세요.";
            }
            else if (value > 0)
            {
                return $"상태이상 저항력  +{value * 100}%";
            }
            else
            {
                return $"상태이상 저항력  -{Mathf.Abs(value) * 100}%";
            }
        }
    }
}