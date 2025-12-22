using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class HealUnitToUnitEffect : UnitToUnitEffect
    {
        [SerializeField] private HealEffectLogic _healEffectLogic;

        public override void Initialize()
        {
            _healEffectLogic = new HealEffectLogic();
        }

        public override string GetDescription()
        {
            return "È¸º¹";
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