using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class DamageUnitToUnitEffect : UnitToUnitEffect
    {
        [SerializeField] private DamageEffectLogic _damageEffectLogic;

        public override void Initialize()
        {
            _damageEffectLogic = new DamageEffectLogic();
        }

        public override string GetDescription()
        {
            return "µ¥¹ÌÁö";
        }

        internal override void Execute(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            _damageEffectLogic.Execute(effectContext, casterUnit, targetUnit);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            _damageEffectLogic.Draw(rect);
        }

        public override int GetNumRows()
        {
            return _damageEffectLogic.GetNumRows();
        }
#endif
    }
}