using System.Collections.Generic;

namespace EvolveThisMatch.Core
{
    public class DestroyShieldEventUnitTrigger : UnitTrigger
    {
        public override string GetLabel()
        {
            return "보호막이 파괴될 시 적용";
        }
    }
}