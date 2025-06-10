using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class GoldGainIncreaseDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"∞ÒµÂ »πµÊ∑Æ¿ª ¡ı∞°°§∞®º“ Ω√ƒ—¡÷ººø‰.";
            }
            else if (value > 0)
            {
                return $"∞ÒµÂ »πµÊ∑Æ  {value * 100}% ¡ı∞°";
            }
            else
            {
                return $"∞ÒµÂ »πµÊ∑Æ  {Mathf.Abs(value) * 100}% ∞®º“";
            }
        }
    }
}