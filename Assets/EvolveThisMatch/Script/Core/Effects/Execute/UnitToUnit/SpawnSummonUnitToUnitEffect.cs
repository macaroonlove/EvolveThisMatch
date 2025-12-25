using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class SpawnSummonUnitToUnitEffect : UnitToUnitEffect, IMutableValueBindingProvider
    {
        [SerializeField] private SpawnSummonEffectLogic _spawnSummonEffectLogic;

        #region MutableValue Ã³¸®
        public override void Initialize()
        {
            _spawnSummonEffectLogic = new SpawnSummonEffectLogic();
            _spawnSummonEffectLogic.Initialize();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            value = null;

            return _spawnSummonEffectLogic != null && _spawnSummonEffectLogic.TryGetBindValue(bindKey, context, out value);
        }
        #endregion

        public override string GetDescription()
        {
            return _spawnSummonEffectLogic.GetDescription();
        }

        internal override void Execute(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            _spawnSummonEffectLogic.Execute(effectContext, casterUnit, targetUnit.transform.position);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            _spawnSummonEffectLogic.Draw(rect);
        }

        public override int GetNumRows()
        {
            return _spawnSummonEffectLogic.GetNumRows();
        }
#endif
    }
}