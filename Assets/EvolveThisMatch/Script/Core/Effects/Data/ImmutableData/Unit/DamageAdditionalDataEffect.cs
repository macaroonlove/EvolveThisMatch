using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class DamageAdditionalDataEffect : ImmutableDataEffect<int>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "피해량을 추가하거나 줄여주세요.";
            }
            else if (_value > 0)
            {
                return $"피해량  {_value} 추가";
            }
            else
            {
                return $"피해량  {Mathf.Abs(_value)} 차감";
            }
        }
    }
}