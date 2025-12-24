using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MaxHPIncreaseDataEffect : IncreaseDataEffect
    {
        public override string GetTitle() => "최대 체력";
    }
}