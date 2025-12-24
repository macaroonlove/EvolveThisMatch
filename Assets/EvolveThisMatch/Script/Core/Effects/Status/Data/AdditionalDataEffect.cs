using UnityEngine;

namespace EvolveThisMatch.Core
{
    public abstract class AdditionalDataEffect : DataEffect<int>, IMutableValueBindingProvider
    {
        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            if (_mutableValue.bindKey == bindKey)
            {
                value = GetValue(context).ToString();
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

        public override int GetValue(EffectContext context)
        {
            return GetValue(_value, context);
        }

        public override int GetValue(EffectContext context, EffectContext contextSub)
        {
            return GetValue(_value, context, contextSub);
        }

        private string FormatDescription(int value)
        {
            if (value == 0)
            {
                return $"{GetTitle()}을(를) 추가하거나 줄여주세요.";
            }
            else if (value > 0)
            {
                return $"{GetTitle()}  {value} 추가";
            }
            else
            {
                return $"{GetTitle()}  {Mathf.Abs(value)} 차감";
            }
        }
    }
}