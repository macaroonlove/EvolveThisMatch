using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class PhysicalPenetrationIncreaseDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"물리 관통력을 증가·감소 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"물리 관통력  {value * 100}% 증가";
            }
            else
            {
                return $"물리 관통력  {value * 100}% 감소";
            }
        }
    }
}