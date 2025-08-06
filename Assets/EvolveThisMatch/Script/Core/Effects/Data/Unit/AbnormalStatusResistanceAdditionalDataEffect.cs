using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AbnormalStatusResistanceAdditionalDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"상태이상 저항력을 추가하거나 줄여주세요.";
            }
            else if (value > 0)
            {
                return $"상태이상 저항력  +{value * 100}%";
            }
            else
            {
                return $"상태이상 저항력  -{Mathf.Abs(value) * 100}%";
            }
        }
    }
}