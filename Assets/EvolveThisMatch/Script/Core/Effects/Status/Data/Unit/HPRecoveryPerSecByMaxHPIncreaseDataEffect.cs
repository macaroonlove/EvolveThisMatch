using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class HPRecoveryPerSecByMaxHPIncreaseDataEffect : IncreaseDataEffect
    {
        public override string GetTitle() => "최대 체력 비례 초당 체력 회복량";
    }
}