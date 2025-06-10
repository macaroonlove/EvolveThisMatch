using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class CriticalHitDamageAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"치명타 데미지를 추가하거나 줄여주세요.";
            }
            else
            {
                return $"치명타 데미지  {value} 추가";
            }
        }
    }
}