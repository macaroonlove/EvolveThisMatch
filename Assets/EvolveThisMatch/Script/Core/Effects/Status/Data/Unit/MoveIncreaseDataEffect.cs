using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MoveIncreaseDataEffect : IncreaseDataEffect
    {
        public override string GetTitle() => "이동속도";
    }
}