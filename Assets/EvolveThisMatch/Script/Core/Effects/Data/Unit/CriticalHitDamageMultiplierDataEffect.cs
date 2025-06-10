using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class CriticalHitDamageMultiplierDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"치명타 데미지를 상승·하락 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"치명타 데미지  {value * 100}% 상승";
            }
            else
            {
                return $"치명타 데미지  {value * 100}% 하락";
            }
        }
    }
}