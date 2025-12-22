using System.Collections.Generic;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// Execute에서 Unit 리스트를 매개변수로 사용하는 Effect
    /// </summary>
    public abstract class BatchUnitEffect : ExecuteEffect
    {
        public abstract void Execute(EffectContext effectContext, List<Unit> targetUnits);
    }
}