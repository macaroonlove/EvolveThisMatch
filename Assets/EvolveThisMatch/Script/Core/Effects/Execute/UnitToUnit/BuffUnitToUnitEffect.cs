using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class BuffUnitToUnitEffect : UnitToUnitEffect, IMutableValueBindingProvider
    {
        [SerializeField] private BuffEffectLogic _buffEffectLogic;

        #region MutableValue 처리
        public override void Initialize()
        {
            _buffEffectLogic = new BuffEffectLogic();
            _buffEffectLogic.Initialize();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            value = null;

            return _buffEffectLogic != null && _buffEffectLogic.TryGetBindValue(bindKey, context, out value);
        }

        public override IEnumerable<Effect> GetChildren() => _buffEffectLogic.GetChildren();
        #endregion

        public override string GetDescription()
        {
            return "버프";
        }

        internal override void Execute(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            _buffEffectLogic.Execute(effectContext, targetUnit);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            _buffEffectLogic.Draw(rect);
        }

        public override int GetNumRows()
        {
            return _buffEffectLogic.GetNumRows();
        }
#endif
    }
}