using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EvolveThisMatch.Core
{
    [System.Serializable]
    public class ApplyTypeByAmountData : IMutableValueBindingProvider
    {
        [SerializeField] private MutableValue _mutableValue;
        [SerializeField] private EApplyType _applyType;
        [SerializeField] private float _amount;

        public EApplyType applyType => _applyType;

        #region MutableValue 처리
        public ApplyTypeByAmountData()
        {
            _mutableValue = new MutableValue();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            if (_mutableValue.bindKey == bindKey)
            {
                value = _mutableValue.GetValueString(_amount, context);
                return true;
            }

            value = null;
            return false;
        }
        #endregion

        #region 최종 Amount 계산
        public float GetAmount(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            float typeValue = _applyType switch
            {
                EApplyType.Basic => 1f,
                EApplyType.Caster_FinalATK => SafeGetStatusValue<AttackAbility>(casterUnit, a => a.finalATK),
                EApplyType.Caster_CurrentHP => SafeGetStatusValue<HealthAbility>(casterUnit, a => a.currentHP),
                EApplyType.Caster_MAXHP => SafeGetStatusValue<HealthAbility>(casterUnit, a => a.finalMaxHP),
                EApplyType.Target_CurrentHP => SafeGetStatusValue<HealthAbility>(targetUnit, a => a.currentHP),
                EApplyType.Target_MAXHP => SafeGetStatusValue<HealthAbility>(targetUnit, a => a.finalMaxHP),

                _ => 0f
            };

            return _mutableValue.GetValue(typeValue * _amount, effectContext);
        }

        private static float SafeGetStatusValue<T>(Unit unit, Func<T, float> getter) where T : Ability
        {
            if (unit == null) return 0;

            var ability = unit.GetAbility<T>();
            return ability != null ? getter(ability) : 0;
        }
        #endregion

        #region 그리기
#if UNITY_EDITOR
        public void Draw(ref Rect rect)
        {
            EffectDrawUtility.DrawBox(ref rect, "적용 방식", valueRect =>
            {
                _applyType = (EApplyType)EditorGUI.EnumPopup(valueRect, _applyType);
            }, boxHeight: 40 + _mutableValue.GetHeight(), valueWidthMargin: 20);


            Color boxColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.20f) : new Color(0, 0, 0, 0.20f);
            var descRect = new Rect(rect.x + 200, rect.y + 1, 500, 18);
            EditorGUI.DrawRect(descRect, boxColor);
            descRect.x += 8;
            descRect.y += 1;
            EditorGUI.LabelField(descRect, $"예상 결과값: {_mutableValue.GetPreviewValue(_amount)}");


            EffectDrawUtility.DrawBoxedMutableValue(ref rect, _mutableValue, "값", valueRect =>
            {
                _amount = EditorGUI.FloatField(valueRect, _amount);
            });
        }

        public float GetNumRows() => 2.5f + _mutableValue.GetNumRows();
#endif
        #endregion
    }
}