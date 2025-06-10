using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ReceiveDamageIncreaseDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"받는 피해량을 증가·감소 시켜주세요.";
            }
            else if (value > 0)
            {
                return $"받는 피해량  {value * 100}% 증가";
            }
            else
            {
                return $"받는 피해량  {value * 100}% 감소";
            }
        }
    }
}