using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ATKMultiplierDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return $"공격력을 상승·하락 시켜주세요.";
            }
            else if (_value > 0)
            {
                return $"공격력  {_value * 100}% 상승";
            }
            else
            {
                return $"공격력  {Mathf.Abs(_value) * 100}% 하락";
            }
        }
    }
}