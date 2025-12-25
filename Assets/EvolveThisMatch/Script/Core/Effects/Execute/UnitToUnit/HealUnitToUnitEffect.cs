using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class HealUnitToUnitEffect : UnitToUnitEffect, IMutableValueBindingProvider
    {
        [SerializeField] private HealEffectLogic _healEffectLogic;

        #region MutableValue Ã³¸®
        public override void Initialize()
        {
            _healEffectLogic = new HealEffectLogic();
            _healEffectLogic.Initialize();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            value = null;

            return _healEffectLogic != null && _healEffectLogic.TryGetBindValue(bindKey, context, out value);
        }
        #endregion

        public override string GetDescription()
        {
            return _healEffectLogic.GetDescription();
        }

        internal override void Execute(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            _healEffectLogic.Execute(effectContext, casterUnit, targetUnit);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            _healEffectLogic.Draw(rect);
        }

        public override int GetNumRows()
        {
            return _healEffectLogic.GetNumRows();
        }
#endif
    }
}