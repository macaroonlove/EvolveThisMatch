using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class CriticalHitChanceAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"치명타 확률을 추가하거나 줄여주세요.";
            }
            else if (value > 0)
            {
                return $"치명타 확률  +{value}%";
            }
            else
            {
                return $"치명타 확률  -{Mathf.Abs(value)}%";
            }
        }
    }
}