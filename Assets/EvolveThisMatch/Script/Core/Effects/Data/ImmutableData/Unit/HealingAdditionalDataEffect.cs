using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class HealingAdditionalDataEffect : ImmutableDataEffect<int>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "회복량을 추가하거나 줄여주세요.";
            }
            else if (_value > 0)
            {
                return $"회복량  {_value} 추가";
            }
            else
            {
                return $"회복량  {Mathf.Abs(_value)} 차감";
            }
        }
    }
}