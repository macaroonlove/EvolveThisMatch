using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ATKIncreaseDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"전투력을 증가·감소 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"전투력  {value * 100}% 증가";
            }
            else
            {
                return $"전투력  {Mathf.Abs(value) * 100}% 감소";
            }
        }
    }
}