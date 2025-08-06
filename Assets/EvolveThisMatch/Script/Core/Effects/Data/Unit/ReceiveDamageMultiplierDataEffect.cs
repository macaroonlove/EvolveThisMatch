using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ReceiveDamageMultiplierDataEffect : DataEffect<float>
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
                return "받는 피해량을 상승·하락 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"받는 피해량  {value * 100}% 상승";
            }
            else
            {
                return $"받는 피해량  {value * 100}% 하락";
            }
        }
    }
}