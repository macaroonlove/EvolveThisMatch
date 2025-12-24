using UnityEngine;

namespace EvolveThisMatch.Core
{
    public abstract class IncreaseDataEffect : DataEffect<float>, IMutableValueBindingProvider
    {
        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            if (_mutableValue.bindKey == bindKey)
            {
                value = (GetValue(context) * 100).ToString();
                return true;
            }

            value = null;
            return false;
        }

        public abstract string GetTitle();

        public override string GetDescription(EffectContext context)
        {
            return FormatDescription(GetValue(_value, context));
        }

        public override string GetDescription()
        {
            return FormatDescription(GetPreviewValue(_value));
        }

        public override float GetValue(EffectContext context)
        {
            return GetValue(_value, context);
        }

        public override float GetValue(EffectContext context, EffectContext contextSub)
        {
            return GetValue(_value, context, contextSub);
        }

        private string FormatDescription(float value)
        {
            if (value == 0)
            {
                return $"{GetTitle()}을(를) 증가·감소 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"{GetTitle()}  {value * 100}% 증가";
            }
            else
            {
                return $"{GetTitle()}  {Mathf.Abs(value) * 100}% 감소";
            }
        }
    }
}