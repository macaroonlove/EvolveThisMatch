using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class ShieldBatchUnitEffect : BatchUnitEffect, IMutableValueBindingProvider
    {
        [SerializeField] private ShieldEffectLogic _shieldErrectLogic;

        #region MutableValue 처리
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
            return "대상 유닛들에게 " + _shieldErrectLogic.GetDescription();
        }

        public override void Execute(EffectContext effectContext, List<Unit> targetUnits)
        {
            foreach (var targetUnit in targetUnits)
            {
                if (targetUnit == null || targetUnit.isDie) continue;

                _shieldErrectLogic.Execute(effectContext, null, targetUnit);
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            _shieldErrectLogic.Draw(rect);

            if (_shieldErrectLogic.HasUnavailableData(EApplyType.Caster_FinalATK, EApplyType.Caster_CurrentHP, EApplyType.Caster_MAXHP))
            {
                EditorGUILayout.HelpBox("ShieldBatchUnitEffect에서는 Caster 기준 계산을 사용할 수 없습니다.", MessageType.Error);
            }
        }

        public override int GetNumRows()
        {
            return _shieldErrectLogic.GetNumRows();
        }
#endif
    }
}