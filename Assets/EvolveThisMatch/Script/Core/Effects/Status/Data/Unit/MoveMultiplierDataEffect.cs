using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MoveMultiplierDataEffect : MultiplierDataEffect
    {
        public override string GetTitle() => "이동속도";
    }
}