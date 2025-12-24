using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AttackCountAdditionalDataEffect : AdditionalDataEffect
    {
        public override string GetTitle() => "최대 공격 가능 대상 수";
    }
}