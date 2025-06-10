using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MoveMultiplierDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"이동속도를 상승·하락 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"이동속도  {value * 100}% 상승";
            }
            else
            {
                return $"이동속도  {Mathf.Abs(value) * 100}% 하락";
            }
        }
    }
}