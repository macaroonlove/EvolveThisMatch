using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class DamageEffectLogic : IMutableValueBindingProvider
    {
        [SerializeField] private MutableValue _repeatCountMutableValue;
        [SerializeField] private ElementalValue _repeatCountElementalValue;
        [SerializeField] private int _repeatCount;
        [SerializeField] private bool _isTick;
        [SerializeField] private MutableValue _tickCycleMutableValue;
        [SerializeField] private ElementalValue _tickCycleElementalValue;
        [SerializeField] private float _tickCycle;
        [SerializeField] private MutableValue _tickCountMutableValue;
        [SerializeField] private ElementalValue _tickCountElementalValue;
        [SerializeField] private int _tickCount;
        [SerializeField] private EDamageType _damageType;

        [SerializeField] private List<ApplyTypeByAmountData> _applyTypeByAmountDatas = new List<ApplyTypeByAmountData>();

        #region MutableValue 처리
        public void Initialize()
        {
            _repeatCountMutableValue = new MutableValue();
            _tickCycleMutableValue = new MutableValue();
            _tickCountMutableValue = new MutableValue();
            _repeatCountElementalValue = new ElementalValue();
            _tickCycleElementalValue = new ElementalValue();
            _tickCountElementalValue = new ElementalValue();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            if (_repeatCountMutableValue.bindKey == bindKey)
            {
                value = _repeatCountMutableValue.GetValueString(_repeatCount, context);
                return true;
            }

            if (_tickCycleMutableValue.bindKey == bindKey)
            {
                value = _tickCycleMutableValue.GetValueString(_tickCycle, context);
                return true;
            }

            if (_tickCountMutableValue.bindKey == bindKey)
            {
                value = _tickCountMutableValue.GetValueString(_tickCount, context);
                return true;
            }

            foreach (var amountData in _applyTypeByAmountDatas)
            {
                if (amountData.TryGetBindValue(bindKey, context, out value))
                {
                    return true;
                }
            }

            value = null;
            return false;
        }
        #endregion

        #region 설명
        public string GetDescription()
        {
            int repeatCount = _repeatCountMutableValue.GetPreviewValue(_repeatCount);
            repeatCount = _repeatCountElementalValue.GetPreviewValue(repeatCount);

            string result = $"{repeatCount}회에 걸쳐";

            if (_isTick)
            {
                float tickCycle = _tickCycleMutableValue.GetPreviewValue(_tickCycle);
                tickCycle = _tickCycleElementalValue.GetPreviewValue(tickCycle);

                int tickCount = _tickCountMutableValue.GetPreviewValue(_tickCount);
                tickCount = _tickCountElementalValue.GetPreviewValue(tickCount);

                float tickTime = tickCycle * tickCount;
                result += $", {tickTime}초 동안 {tickCycle}초 마다";
            }

            return result + " 데미지를 가합니다.";
        }
        #endregion

        public void Execute(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            int damage = GetAmount(effectContext, casterUnit, targetUnit);

            Execute_RepeatCount(effectContext, casterUnit, targetUnit, damage);
        }

        #region 데미지 계산
        private int GetAmount(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            float totalAmount = 0;

            foreach (var applyTypeByAmountData in _applyTypeByAmountDatas)
            {
                var amount = applyTypeByAmountData.GetAmount(effectContext, casterUnit, targetUnit);
                totalAmount += applyTypeByAmountData.GetElementalBuffAmount(casterUnit, amount);
            }

            return (int)totalAmount;
        }
        #endregion

        #region 피해 횟수
        private void Execute_RepeatCount(EffectContext context, Unit casterUnit, Unit targetUnit, int damage)
        {
            int repeatCount = _repeatCountMutableValue.GetValue(_repeatCount, context);
            repeatCount = _repeatCountElementalValue.GetValue(repeatCount);

            if (repeatCount > 1)
            {
                for (int i = 0; i < repeatCount; i++)
                {
                    if (targetUnit.isDie) return;

                    Execute_Tick(context, casterUnit, targetUnit, damage);
                }
            }
            else
            {
                Execute_Tick(context, casterUnit, targetUnit, damage);
            }
        }
        #endregion

        #region 지속 피해
        private void Execute_Tick(EffectContext context, Unit casterUnit, Unit targetUnit, int damage)
        {
            if (_isTick)
            {
                targetUnit.StartCoroutine(CoExecute_Tick(context, casterUnit, targetUnit, damage));
            }
            else
            {
                Execute_DamageType(casterUnit, targetUnit, damage);
            }
        }

        private IEnumerator CoExecute_Tick(EffectContext context, Unit casterUnit, Unit targetUnit, int damage)
        {
            var tickCycle = _tickCycleMutableValue.GetValue(_tickCycle, context);
            tickCycle = _tickCycleElementalValue.GetValue(tickCycle);
            var tickCount = _tickCountMutableValue.GetValue(_tickCount, context);
            tickCount = _tickCountElementalValue.GetValue(tickCount);

            var wfs = new WaitForSeconds(tickCycle);

            for (int i = 0; i < tickCount; i++)
            {
                if (targetUnit.isDie) yield break;

                Execute_DamageType(casterUnit, targetUnit, damage);
                yield return wfs;
            }
        }
        #endregion

        #region 피해 타입
        private void Execute_DamageType(Unit casterUnit, Unit targetUnit, int damage)
        {
            int initiator = casterUnit == null ? 0 : casterUnit.id;

            if (_damageType == EDamageType.TrueDamage)
            {
                targetUnit.GetAbility<HitAbility>().Hit(damage, initiator);
            }
            else
            {
                targetUnit.GetAbility<HitAbility>().Hit(damage, _damageType, initiator);
            }
        }
        #endregion

        #region 그리기
#if UNITY_EDITOR
        public void Draw(Rect rect)
        {
            #region 피해 횟수
            EffectDrawUtility.DrawBoxedScaledValue(ref rect, _repeatCountMutableValue, _repeatCountElementalValue, "피해 횟수", valueRect =>
            {
                _repeatCount = EditorGUI.IntField(valueRect, _repeatCount);
                if (_repeatCount <= 0) _repeatCount = 1;
            });
            #endregion

            #region 주기마다 피해
            rect.y += 20;
            EffectDrawUtility.DrawRow(ref rect, "주기마다 피해 사용 여부", valueRect =>
            {
                _isTick = EditorGUI.Toggle(valueRect, _isTick);
            });

            if (_isTick)
            {
                #region 주기(초)
                EffectDrawUtility.DrawBoxedScaledValue(ref rect, _tickCycleMutableValue, _tickCycleElementalValue, "주기(초)", valueRect =>
                {
                    _tickCycle = EditorGUI.FloatField(valueRect, _tickCycle);
                });
                #endregion

                #region 주기마다 피해 횟수
                rect.y += 5;
                EffectDrawUtility.DrawBoxedScaledValue(ref rect, _tickCountMutableValue, _tickCountElementalValue, "주기마다 피해 횟수", valueRect =>
                {
                    _tickCount = EditorGUI.IntField(valueRect, _tickCount);
                });
                #endregion                
            }
            else
            {
                _tickCycleMutableValue.ForceDisable();
                _tickCountMutableValue.ForceDisable();
            }
            #endregion

            #region 데미지 계산
            rect.y += 20;
            EffectDrawUtility.DrawRow(ref rect, "데미지 타입", valueRect =>
            {
                _damageType = (EDamageType)EditorGUI.EnumPopup(valueRect, _damageType);
            });

            rect.y += 5;

            EffectDrawUtility.DrawRow(ref rect, "", valueRect =>
            {
                if (GUI.Button(valueRect, "추가"))
                {
                    _applyTypeByAmountDatas.Add(new ApplyTypeByAmountData());
                }
            }, 0);

            rect.y += 5;

            for (int i = 0; i < _applyTypeByAmountDatas.Count; i++)
            {
                float rowStartY = rect.y;
                _applyTypeByAmountDatas[i].Draw(ref rect);

                var deleteRect = new Rect(rect.xMax - 18, rowStartY, 18, 18);
                if (GUI.Button(deleteRect, "X"))
                {
                    _applyTypeByAmountDatas.RemoveAt(i);
                    break;
                }

                rect.y += 5;
            }
            #endregion
        }

        public int GetNumRows()
        {
            float rowNum = 1;

            if (_isTick)
            {
                rowNum += 4;
            }

            foreach (var amountData in _applyTypeByAmountDatas)
            {
                rowNum += amountData.GetNumRows();
            }
            rowNum += _repeatCountMutableValue.GetNumRows();
            rowNum += _repeatCountElementalValue.GetNumRows();
            rowNum += _tickCycleMutableValue.GetNumRows();
            rowNum += _tickCycleElementalValue.GetNumRows();
            rowNum += _tickCountMutableValue.GetNumRows();
            rowNum += _tickCountElementalValue.GetNumRows();

            return (int)rowNum;
        }

        /// <summary>
        /// 사용 불가능한 데이터가 있는지 검사
        /// </summary>
        public bool HasUnavailableData(params EApplyType[] ignoreTypes)
        {
            foreach (var data in _applyTypeByAmountDatas)
            {
                foreach (var ignore in ignoreTypes)
                {
                    if (data.applyType == ignore)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
#endif
        #endregion
    }
}