using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class PhysicalPenetrationAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"물리 관통력을 추가하거나 줄여주세요.";
            }
            else
            {
                return $"물리 관통력  {value} 추가";
            }
        }
    }
}