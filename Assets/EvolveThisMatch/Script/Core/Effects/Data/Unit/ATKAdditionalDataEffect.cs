using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ATKAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"공격력을 추가하거나 줄여주세요.";
            }
            else if (value > 0)
            {
                return $"공격력  {value} 추가";
            }
            else
            {
                return $"공격력  {Mathf.Abs(value)} 차감";
            }
        }
    }
}