using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class DamageIncreaseDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "피해량을 증가·감소 시켜주세요.";
            }
            else if (_value > 0)
            {
                return $"피해량  {_value * 100}% 증가";
            }
            else
            {
                return $"피해량  {_value * 100}% 감소";
            }
        }
    }
}