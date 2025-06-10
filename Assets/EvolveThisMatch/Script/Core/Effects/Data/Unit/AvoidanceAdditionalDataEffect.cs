using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AvoidanceAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"회피 확률을 추가하거나 줄여주세요.";
            }
            else if (value > 0)
            {
                return $"회피 확률  {value} 추가";
            }
            else
            {
                return $"회피 확률  {Mathf.Abs(value)} 차감";
            }
        }
    }
}