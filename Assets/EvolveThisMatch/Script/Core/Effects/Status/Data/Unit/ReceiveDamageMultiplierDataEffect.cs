using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ReceiveDamageMultiplierDataEffect : MultiplierDataEffect
    {
        public override string GetTitle() => "받는 피해량";
    }
}