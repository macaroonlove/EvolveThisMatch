using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AbnormalStatusResistanceAdditionalDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "상태이상 저항력을 추가하거나 줄여주세요.";
            }
            else if (_value > 0)
            {
                return $"상태이상 저항력  +{_value * 100}%";
            }
            else
            {
                return $"상태이상 저항력  -{Mathf.Abs(_value) * 100}%";
            }
        }
    }
}