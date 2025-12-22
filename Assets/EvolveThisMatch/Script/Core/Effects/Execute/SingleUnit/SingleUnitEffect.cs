namespace EvolveThisMatch.Core
{
    /// <summary>
    /// Execute에서 시전자 Unit 매개변수를 사용하는 Effect
    /// </summary>
    public abstract class SingleUnitEffect : ExecuteEffect
    {
        public abstract void Execute(EffectContext effectContext, Unit casterUnit);
    }
}