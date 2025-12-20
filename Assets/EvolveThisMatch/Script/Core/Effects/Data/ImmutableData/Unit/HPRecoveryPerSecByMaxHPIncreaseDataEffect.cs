using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class HPRecoveryPerSecByMaxHPIncreaseDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "최대 체력 비례 초당 체력 회복량을 증가·감소 시켜주세요.";
            }
            else if (_value > 0)
            {
                return $"최대 체력 비례 초당 체력 회복량  {_value * 100}% 증가";
            }
            else
            {
                return $"최대 체력 비례 초당 체력 회복량  {_value * 100}% 감소";
            }
        }
    }
}