using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ManaRecoveryPerSecMultiplierDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"초당 마나 회복량을 상승·하락 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"초당 마나 회복량  {value * 100}% 상승";
            }
            else
            {
                return $"초당 마나 회복량  {Mathf.Abs(value) * 100}% 하락";
            }
        }
    }
}