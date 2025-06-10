using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class PhysicalResistanceIncreaseDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"물리 저항력을 증가·감소 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"물리 저항력  {value * 100}% 증가";
            }
            else
            {
                return $"물리 저항력  {value * 100}% 감소";
            }
        }
    }
}