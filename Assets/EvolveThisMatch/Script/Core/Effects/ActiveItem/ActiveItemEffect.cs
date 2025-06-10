using System.Collections.Generic;

namespace EvolveThisMatch.Core
{
    public abstract class ActiveItemEffect : Effect
    {
        public abstract void Execute(List<Unit> targetUnits);
    }
}