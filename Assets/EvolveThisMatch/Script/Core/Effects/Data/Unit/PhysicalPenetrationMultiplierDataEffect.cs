using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class PhysicalPenetrationMultiplierDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"물리 관통력을 상승·하락 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"물리 관통력  {value * 100}% 상승";
            }
            else
            {
                return $"물리 관통력  {value * 100}% 하락";
            }
        }
    }
}