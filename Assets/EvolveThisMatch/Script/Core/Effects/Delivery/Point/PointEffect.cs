using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 특정 지점으로 전달
    /// </summary>
    public abstract class PointEffect : DeliveryEffect
    {
        public abstract void Deliver(EffectContext effectContext, Unit casterUnit, Vector3 targetVector);
    }
}