using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ATKAdditionalDataEffect : ImmutableDataEffect<int>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "공격력을 추가하거나 줄여주세요.";
            }
            else if (_value > 0)
            {
                return $"공격력  {_value} 추가";
            }
            else
            {
                return $"공격력  {Mathf.Abs(_value)} 차감";
            }
        }
    }
}