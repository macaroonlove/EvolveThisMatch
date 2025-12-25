using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class HealBatchUnitEffect : BatchUnitEffect, IMutableValueBindingProvider
    {
        [SerializeField] private HealEffectLogic _healEffectLogic;

        #region MutableValue 처리
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
            return "대상 유닛들을 " + _healEffectLogic.GetDescription();
        }

        public override void Execute(EffectContext effectContext, List<Unit> targetUnits)
        {
            foreach (var targetUnit in targetUnits)
            {
                if (targetUnit == null || targetUnit.isDie) continue;

                _healEffectLogic.Execute(effectContext, null, targetUnit);
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            _healEffectLogic.Draw(rect);

            if (_healEffectLogic.HasUnavailableData(EApplyType.Caster_FinalATK, EApplyType.Caster_CurrentHP, EApplyType.Caster_MAXHP))
            {
                EditorGUILayout.HelpBox("HealBatchUnitEffect에서는 Caster 기준 계산을 사용할 수 없습니다.", MessageType.Error);
            }
        }

        public override int GetNumRows()
        {
            return _healEffectLogic.GetNumRows();
        }
#endif
    }
}