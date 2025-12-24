using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MagicResistanceIncreaseDataEffect : IncreaseDataEffect
    {
        public override string GetTitle() => "마법 저항력";
    }
}