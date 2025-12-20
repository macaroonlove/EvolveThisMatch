using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MagicResistanceIncreaseDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "마법 저항력을 증가·감소 시켜주세요.";
            }
            else if (_value > 0)
            {
                return $"마법 저항력  {_value * 100}% 증가";
            }
            else
            {
                return $"마법 저항력  {_value * 100}% 감소";
            }
        }
    }
}