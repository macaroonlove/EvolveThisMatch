using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class GoldGainIncreaseDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "∞ÒµÂ »πµÊ∑Æ¿ª ¡ı∞°°§∞®º“ Ω√ƒ—¡÷ººø‰.";
            }
            else if (_value > 0)
            {
                return $"∞ÒµÂ »πµÊ∑Æ  {_value * 100}% ¡ı∞°";
            }
            else
            {
                return $"∞ÒµÂ »πµÊ∑Æ  {Mathf.Abs(_value) * 100}% ∞®º“";
            }
        }
    }
}