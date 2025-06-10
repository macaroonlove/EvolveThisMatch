using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MoveIncreaseDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"이동속도를 증가·감소 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"이동속도  {value * 100}% 증가";
            }
            else
            {
                return $"이동속도  {Mathf.Abs(value) * 100}% 감소";
            }
        }
    }
}