using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class HealingMultiplierDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "회복량을 상승·하락 시켜주세요.";
            }
            else if (_value > 0)
            {
                return $"회복량  {_value * 100}% 상승";
            }
            else
            {
                return $"회복량  {_value * 100}% 하락";
            }
        }
    }
}