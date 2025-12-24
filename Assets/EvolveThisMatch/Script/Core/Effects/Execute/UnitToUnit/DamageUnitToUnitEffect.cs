using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class DamageUnitToUnitEffect : UnitToUnitEffect, IMutableValueBindingProvider
    {
        [SerializeField] private DamageEffectLogic _damageEffectLogic;

        #region MutableValue 처리
        public override void Initialize()
        {
            _damageEffectLogic = new DamageEffectLogic();
            _damageEffectLogic.Initialize();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            value = null;

            return _damageEffectLogic != null && _damageEffectLogic.TryGetBindValue(bindKey, context, out value);
        }
        #endregion

        public override string GetDescription()
        {
            return "데미지";
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