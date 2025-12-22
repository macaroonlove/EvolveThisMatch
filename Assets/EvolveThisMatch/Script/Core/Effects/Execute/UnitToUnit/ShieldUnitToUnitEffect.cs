using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class ShieldUnitToUnitEffect : UnitToUnitEffect
    {
        [SerializeField] private ShieldEffectLogic _shieldErrectLogic;

        public override void Initialize()
        {
            _shieldErrectLogic = new ShieldEffectLogic();
        }

        public override string GetDescription()
        {
            return "º¸È£¸·";
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