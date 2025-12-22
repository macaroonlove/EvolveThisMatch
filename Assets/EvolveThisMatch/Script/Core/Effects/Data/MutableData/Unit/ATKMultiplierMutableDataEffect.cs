using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ATKMultiplierMutableDataEffect : MutableDataEffect<float>
    {
        public override string GetDescription()
        {
            return FormatDescription(GetPreviewValue(_value));
        }

        public override string GetDescription(EffectContext context)
        {
            return FormatDescription(GetValue(context));
        }

        public override float GetValue(EffectContext context)
        {
            int scaleBase = context.GetScaleValue(_scaleBase);

            return _value + (scaleBase * _scaleFactor);
        }

        private string FormatDescription(float value)
        {
            if (value == 0)
            {
                return $"전투력을 상승·하락 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"전투력  {value * 100}% 상승";
            }
            else
            {
                return $"전투력  {Mathf.Abs(value) * 100}% 하락";
            }
        }
    }
}