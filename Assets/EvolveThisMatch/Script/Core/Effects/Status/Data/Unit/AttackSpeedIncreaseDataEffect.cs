using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AttackSpeedIncreaseDataEffect : IncreaseDataEffect
    {
        public override string GetTitle() => "공격 간격";
    }
}