using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class HealBatchUnitEffect : BatchUnitEffect
    {
        [SerializeField] private HealEffectLogic _healEffectLogic;

        public override void Initialize()
        {
            _healEffectLogic = new HealEffectLogic();
        }

        public override string GetDescription()
        {
            return "대상 유닛들에게 회복 적용";
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