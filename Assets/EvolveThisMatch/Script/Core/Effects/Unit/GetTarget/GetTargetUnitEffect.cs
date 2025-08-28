using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public abstract class GetTargetUnitEffect : UnitEffect
    {
        [SerializeField] protected GetTargetData _getTargetData = new GetTargetData();

        public List<Unit> GetTarget(Unit casterUnit)
        {
            return _getTargetData.GetTarget(casterUnit);
        }
    }
}