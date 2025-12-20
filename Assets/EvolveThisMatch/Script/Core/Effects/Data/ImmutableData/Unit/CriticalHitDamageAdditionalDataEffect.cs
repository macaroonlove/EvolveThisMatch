using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class CriticalHitDamageAdditionalDataEffect : ImmutableDataEffect<int>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "치명타 데미지를 추가하거나 줄여주세요.";
            }
            else if (_value > 0)
            {
                return $"치명타 데미지  {_value} 추가";
            }
            else
            {
                return $"치명타 데미지  {Mathf.Abs(_value)} 차감";
            }
        }
    }
}