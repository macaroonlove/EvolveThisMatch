using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MagicPenetrationAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"마법 관통력을 추가하거나 줄여주세요.";
            }
            else
            {
                return $"마법 관통력  {value} 추가";
            }
        }
    }
}