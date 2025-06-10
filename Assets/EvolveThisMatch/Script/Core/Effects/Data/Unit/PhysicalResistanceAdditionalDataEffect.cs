using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class PhysicalResistanceAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"물리 저항력을 추가하거나 줄여주세요.";
            }
            else
            {
                return $"물리 저항력  {value} 추가";
            }
        }
    }
}