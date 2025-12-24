using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class SkillCooldownIncreaseDataEffect : IncreaseDataEffect
    {
        public override string GetTitle() => "스킬 가속";
    }
}