using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class ShieldUnitToUnitEffect : UnitToUnitEffect, IMutableValueBindingProvider
    {
        [SerializeField] private ShieldEffectLogic _shieldErrectLogic;

        #region MutableValue Ã³¸®
        public override void Initialize()
        {
            _shieldErrectLogic = new ShieldEffectLogic();
            _shieldErrectLogic.Initialize();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            value = null;

            return _shieldErrectLogic != null && _shieldErrectLogic.TryGetBindValue(bindKey, context, out value);
        }
        #endregion

        public override string GetDescription()
        {
            return _shieldErrectLogic.GetDescription();
        }

        internal override void Execute(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            _shieldErrectLogic.Execute(effectContext, casterUnit, targetUnit);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            _shieldErrectLogic.Draw(rect);
        }

        public override int GetNumRows()
        {
            return _shieldErrectLogic.GetNumRows();
        }
#endif
    }
}