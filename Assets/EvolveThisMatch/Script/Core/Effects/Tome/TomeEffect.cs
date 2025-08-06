using System.Collections.Generic;

namespace EvolveThisMatch.Core
{
    public abstract class TomeEffect : Effect
    {
        public abstract void Execute(List<Unit> targetUnits, int level);
    }
}