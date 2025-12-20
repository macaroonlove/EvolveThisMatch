using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class CriticalHitDamageIncreaseDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "치명타 데미지를 증가·감소 시켜주세요.";
            }
            else if (_value > 0)
            {
                return $"치명타 데미지  {_value * 100}% 증가";
            }
            else
            {
                return $"치명타 데미지  {_value * 100}% 감소";
            }
        }
    }
}