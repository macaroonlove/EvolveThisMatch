using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AvoidanceAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            return FormatDescription(_value);
        }

        public override string GetDescription(int level)
        {
            return FormatDescription(GetValue(level));
        }

        public override int GetValue(int level)
        {
            return _value + 1 * (level - 1);
        }

        private string FormatDescription(int value)
        {
            if (value == 0)
            {
                return "회피 확률을 추가하거나 줄여주세요.";
            }
            else if (value > 0)
            {
                return $"회피 확률  +{value}%";
            }
            else
            {
                return $"회피 확률  -{Mathf.Abs(value)}%";
            }
        }
    }
}