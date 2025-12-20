using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ReceiveDamageIncreaseDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "받는 피해량을 증가·감소 시켜주세요.";
            }
            else if (_value > 0)
            {
                return $"받는 피해량  {_value * 100}% 증가";
            }
            else
            {
                return $"받는 피해량  {_value * 100}% 감소";
            }
        }
    }
}