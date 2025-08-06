using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ATKAdditionalDataEffect : DataEffect<int>
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
                return "공격력을 추가하거나 줄여주세요.";
            }
            else if (value > 0)
            {
                return $"공격력  {value} 추가";
            }
            else
            {
                return $"공격력  {Mathf.Abs(value)} 차감";
            }
        }
    }
}