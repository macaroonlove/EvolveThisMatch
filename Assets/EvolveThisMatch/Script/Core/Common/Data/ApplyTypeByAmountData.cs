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
        [SerializeField] private ElementalValue _elementalValue;
        [SerializeField] private EApplyType _applyType;
        [SerializeField] private float _amount;

        public EApplyType applyType => _applyType;

        #region MutableValue 처리
        public ApplyTypeByAmountData()
        {
            _mutableValue = new MutableValue();
            _elementalValue = new ElementalValue();
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

            float amount = _mutableValue.GetValue(typeValue * _amount, effectContext);
            amount = _elementalValue.GetValue(amount);
            return amount;
        }

        public float GetElementalBuffAmount(Unit casterUnit, float amount)
        {
            var buffAbility = casterUnit.GetAbility<BuffAbility>();

            float increase = 1;

            switch (_elementalValue.elementalType)
            {
                case EElementalType.Divine:
                    foreach (var instance in buffAbility.ElementalDivineIncreaseDataEffects)
                    {
                        increase += instance.effect.GetValue(instance.context);
                    }
                    break;
                case EElementalType.Dark:
                    foreach (var instance in buffAbility.ElementalDarkIncreaseDataEffects)
                    {
                        increase += instance.effect.GetValue(instance.context);
                    }
                    break;
                case EElementalType.Fire:
                    foreach (var instance in buffAbility.ElementalFireIncreaseDataEffects)
                    {
                        increase += instance.effect.GetValue(instance.context);
                    }
                    break;
                case EElementalType.Water:
                    foreach (var instance in buffAbility.ElementalWaterIncreaseDataEffects)
                    {
                        increase += instance.effect.GetValue(instance.context);
                    }
                    break;
                case EElementalType.Earth:
                    foreach (var instance in buffAbility.ElementalEarthIncreaseDataEffects)
                    {
                        increase += instance.effect.GetValue(instance.context);
                    }
                    break;
                case EElementalType.Wind:
                    foreach (var instance in buffAbility.ElementalWindIncreaseDataEffects)
                    {
                        increase += instance.effect.GetValue(instance.context);
                    }
                    break;
                case EElementalType.Thunder:
                    foreach (var instance in buffAbility.ElementalThunderIncreaseDataEffects)
                    {
                        increase += instance.effect.GetValue(instance.context);
                    }
                    break;
            }

            amount *= increase;

            return amount;
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
            }, boxHeight: 40 + _mutableValue.GetHeight() + _elementalValue.GetHeight(), valueWidthMargin: 20);


            Color boxColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.20f) : new Color(0, 0, 0, 0.20f);
            var descRect = new Rect(rect.x + 200, rect.y + 1, 500, 18);
            EditorGUI.DrawRect(descRect, boxColor);
            descRect.x += 8;
            descRect.y += 1;
            var previewAmount = _mutableValue.GetPreviewValue(_amount);
            previewAmount = _elementalValue.GetPreviewValue(previewAmount);
            EditorGUI.LabelField(descRect, $"예상 결과값: {GetApplyTypeString()} {previewAmount}%");


            EffectDrawUtility.DrawBoxedScaledValue(ref rect, _mutableValue, _elementalValue, "값", valueRect =>
            {
                _amount = EditorGUI.FloatField(valueRect, _amount);
            });
        }

        private string GetApplyTypeString()
        {
            switch (_applyType)
            {
                case EApplyType.Basic:
                    return "기본값의";
                case EApplyType.Caster_FinalATK:
                    return "시전자의 전투력의";
                case EApplyType.Caster_CurrentHP:
                    return "시전자의 현재 체력의";
                case EApplyType.Caster_MAXHP:
                    return "시전자의 최대 체력의";
                case EApplyType.Target_CurrentHP:
                    return "타겟의 현재 체력의";
                case EApplyType.Target_MAXHP:
                    return "타겟의 최대 체력의";
                default:
                    break;
            }
            return "";
        }

        public float GetNumRows() => 3 + _mutableValue.GetNumRows() + _elementalValue.GetNumRows();
#endif
        #endregion
    }
}