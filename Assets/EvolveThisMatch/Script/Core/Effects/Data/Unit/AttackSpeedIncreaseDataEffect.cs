using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AttackSpeedIncreaseDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"공격간격를 증가·감소 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"공격간격  {value * 100}% 감소";
            }
            else
            {
                return $"공격간격  {Mathf.Abs(value) * 100}% 증가";
            }
        }
    }
}