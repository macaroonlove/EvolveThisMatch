using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MagicResistanceIncreaseDataEffect : DataEffect<float>
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
                return "마법 저항력을 증가·감소 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"마법 저항력  {value * 100}% 증가";
            }
            else
            {
                return $"마법 저항력  {value * 100}% 감소";
            }
        }
    }
}