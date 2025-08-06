namespace EvolveThisMatch.Core
{
    public abstract class AlwaysEffect : Effect
    {
        public abstract void Execute(Unit casterUnit, int level = 1);
    }
}