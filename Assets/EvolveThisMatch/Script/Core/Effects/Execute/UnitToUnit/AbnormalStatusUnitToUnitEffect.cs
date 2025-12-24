using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AbnormalStatusUnitToUnitEffect : UnitToUnitEffect, IMutableValueBindingProvider
    {
        [SerializeField] private AbnormalStatusEffectLogic _abnormalStatusEffectLogic;

        #region MutableValue 처리
        public override void Initialize()
        {
            _abnormalStatusEffectLogic = new AbnormalStatusEffectLogic();
            _abnormalStatusEffectLogic.Initialize();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            value = null;

            return _abnormalStatusEffectLogic != null && _abnormalStatusEffectLogic.TryGetBindValue(bindKey, context, out value);
        }

        public override IEnumerable<Effect> GetChildren() => _abnormalStatusEffectLogic.GetChildren();
        #endregion

        public override string GetDescription()
        {
            return "상태이상";
        }

        internal override void Execute(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            _abnormalStatusEffectLogic.Execute(effectContext, targetUnit);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            _abnormalStatusEffectLogic.Draw(rect);
        }

        public override int GetNumRows()
        {
            return _abnormalStatusEffectLogic.GetNumRows();
        }
#endif
    }
}