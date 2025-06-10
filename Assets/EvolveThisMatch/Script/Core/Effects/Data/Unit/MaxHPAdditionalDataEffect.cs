using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MaxHPAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"최대 체력을 추가하거나 줄여주세요.";
            }
            else
            {
                return $"최대 체력  {value} 추가";
            }
        }
    }
}