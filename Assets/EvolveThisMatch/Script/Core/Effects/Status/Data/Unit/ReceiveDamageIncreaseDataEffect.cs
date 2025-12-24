using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ReceiveDamageIncreaseDataEffect : IncreaseDataEffect
    {
        public override string GetTitle() => "받는 피해량";
    }
}