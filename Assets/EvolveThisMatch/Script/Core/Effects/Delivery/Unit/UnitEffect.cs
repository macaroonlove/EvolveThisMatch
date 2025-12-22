namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 유닛에게 전달
    /// </summary>
    public abstract class UnitEffect : DeliveryEffect
    {
        public abstract void Deliver(EffectContext effectContext, Unit casterUnit, Unit targetUnit);
    }
}