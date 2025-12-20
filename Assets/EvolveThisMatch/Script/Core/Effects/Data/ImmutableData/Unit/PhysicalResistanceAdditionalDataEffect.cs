using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class PhysicalResistanceAdditionalDataEffect : ImmutableDataEffect<int>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "물리 저항력을 추가하거나 줄여주세요.";
            }
            else if (_value > 0)
            {
                return $"물리 저항력  {_value} 추가";
            }
            else
            {
                return $"물리 저항력  {Mathf.Abs(_value)} 차감";
            }
        }
    }
}