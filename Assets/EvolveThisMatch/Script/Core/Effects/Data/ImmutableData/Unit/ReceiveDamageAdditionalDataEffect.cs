using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ReceiveDamageAdditionalDataEffect : ImmutableDataEffect<int>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "받는 피해량을 추가하거나 줄여주세요.";
            }
            else if (_value > 0)
            {
                return $"받는 피해량  {_value} 추가";
            }
            else
            {
                return $"받는 피해량  {Mathf.Abs(_value)} 차감";
            }
        }
    }
}