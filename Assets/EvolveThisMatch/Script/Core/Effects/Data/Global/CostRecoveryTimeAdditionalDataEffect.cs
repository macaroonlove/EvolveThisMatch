using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class CostRecoveryTimeAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"코스트 중가 속도를 추가하거나 줄여주세요.";
            }
            else if (value > 0)
            {
                return $"코스트 중가 속도  {value} 차감";
            }
            else
            {
                return $"코스트 중가 속도  {Mathf.Abs(value)} 추가";
            }
        }
    }
}