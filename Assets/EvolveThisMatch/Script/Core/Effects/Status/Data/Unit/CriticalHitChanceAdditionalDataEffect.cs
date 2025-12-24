using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class CriticalHitChanceAdditionalDataEffect : AdditionalDataEffect
    {
        public override string GetTitle() => "치명타 확률";
    }
}