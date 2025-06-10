using System.Collections.Generic;

namespace EvolveThisMatch.Core
{
    public class AlwaysUnitTrigger : UnitTrigger
    {
        public override string GetLabel()
        {
            return "상시 적용";
        }
    }
}