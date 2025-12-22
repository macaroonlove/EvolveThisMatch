namespace EvolveThisMatch.Core
{
    /// <summary>
    /// Execute에서 시전자 Unit과 타겟 Unit을 매개변수를 사용하는 Effect
    /// </summary>
    public abstract class UnitToUnitEffect : ExecuteEffect
    {
        internal abstract void Execute(EffectContext effectContext, Unit casterUnit, Unit targetUnit);
    }
}