using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AttackCountAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"최대 공격 가능 대상 수를 추가하거나 줄여주세요.";
            }
            else if (value > 0)
            {
                return $"최대 공격 가능 대상 수  {value} 추가";
            }
            else
            {
                return $"최대 공격 가능 대상 수  {Mathf.Abs(value)} 차감";
            }
        }
    }
}