using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AttackSpeedMultiplierDataEffect : MultiplierDataEffect
    {
        public override string GetTitle() => "공격 간격";
    }
}