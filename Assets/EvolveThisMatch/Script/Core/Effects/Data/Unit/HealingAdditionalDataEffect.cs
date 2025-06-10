using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class HealingAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"회복량을 추가하거나 줄여주세요.";
            }
            else
            {
                return $"회복량  {value} 추가";
            }
        }
    }
}