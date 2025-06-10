using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class DamageAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"피해량을 추가하거나 줄여주세요.";
            }
            else
            {
                return $"피해량  {value} 추가";
            }
        }
    }
}