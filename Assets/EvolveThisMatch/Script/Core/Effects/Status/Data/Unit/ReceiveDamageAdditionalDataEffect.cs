using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ReceiveDamageAdditionalDataEffect : AdditionalDataEffect
    {
        public override string GetTitle() => "받는 피해량";
    }
}