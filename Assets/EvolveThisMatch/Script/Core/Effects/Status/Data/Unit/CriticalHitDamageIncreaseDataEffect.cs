using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class CriticalHitDamageIncreaseDataEffect : IncreaseDataEffect
    {
        public override string GetTitle() => "치명타 데미지";
    }
}