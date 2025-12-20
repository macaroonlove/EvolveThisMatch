using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AvoidanceAdditionalDataEffect : ImmutableDataEffect<int>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "회피 확률을 추가하거나 줄여주세요.";
            }
            else if (_value > 0)
            {
                return $"회피 확률  +{_value}%";
            }
            else
            {
                return $"회피 확률  -{Mathf.Abs(_value)}%";
            }
        }
    }
}