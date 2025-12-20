using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MoveMultiplierDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "이동속도를 상승·하락 시켜주세요.";
            }
            else if (_value > 0)
            {
                return $"이동속도  {_value * 100}% 상승";
            }
            else
            {
                return $"이동속도  {Mathf.Abs(_value) * 100}% 하락";
            }
        }
    }
}