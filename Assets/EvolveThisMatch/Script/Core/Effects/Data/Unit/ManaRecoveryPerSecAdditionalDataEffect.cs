using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ManaRecoveryPerSecAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"초당 마나 회복량을 추가하거나 줄여주세요.";
            }
            else if (value > 0)
            {
                return $"초당 마나 회복량  {value} 추가";
            }
            else
            {
                return $"초당 마나 회복량  {Mathf.Abs(value)} 차감";
            }
        }
    }
}