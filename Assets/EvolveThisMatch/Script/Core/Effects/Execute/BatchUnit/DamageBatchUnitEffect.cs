using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class DamageBatchUnitEffect : BatchUnitEffect, IMutableValueBindingProvider
    {
        [SerializeField] private DamageEffectLogic _damageEffectLogic;

        #region MutableValue 처리
        public override void Initialize()
        {
            _damageEffectLogic = new DamageEffectLogic();
            _damageEffectLogic.Initialize();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            value = null;

            return _damageEffectLogic != null && _damageEffectLogic.TryGetBindValue(bindKey, context, out value);
        }
        #endregion

        public override string GetDescription()
        {
            return "대상 유닛들에게 데미지 적용";
        }

        public override void Execute(EffectContext effectContext, List<Unit> targetUnits)
        {
            foreach (var targetUnit in targetUnits)
            {
                if (targetUnit == null || targetUnit.isDie) continue;

                _damageEffectLogic.Execute(effectContext, null, targetUnit);
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            _damageEffectLogic.Draw(rect);

            if (_damageEffectLogic.HasUnavailableData(EApplyType.Caster_FinalATK, EApplyType.Caster_CurrentHP, EApplyType.Caster_MAXHP))
            {
                EditorGUILayout.HelpBox("DamageBatchUnitEffect에서는 Caster 기준 계산을 사용할 수 없습니다.", MessageType.Error);
            }
        }

        public override int GetNumRows()
        {
            return _damageEffectLogic.GetNumRows();
        }
#endif
    }
}