using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MaxHPMultiplierDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"최대 체력을 상승·하락 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"최대 체력  {value * 100}% 상승";
            }
            else
            {
                return $"최대 체력  {value * 100}% 하락";
            }
        }
    }
}