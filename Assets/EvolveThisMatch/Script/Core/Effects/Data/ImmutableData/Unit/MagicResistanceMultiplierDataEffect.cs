using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MagicResistanceMultiplierDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "마법 저항력을 상승·하락 시켜주세요.";
            }
            else if (_value > 0)
            {
                return $"마법 저항력  {_value * 100}% 상승";
            }
            else
            {
                return $"마법 저항력  {_value * 100}% 하락";
            }
        }
    }
}