using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class DamageMultiplierDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"피해량을 상승·하락 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"피해량  {value * 100}% 상승";
            }
            else
            {
                return $"피해량  {value * 100}% 하락";
            }
        }
    }
}