using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ShieldEffectLogic : IMutableValueBindingProvider
    {
        [SerializeField] private MutableValue _repeatCountMutableValue;
        [SerializeField] private int _repeatCount;
        [SerializeField] private bool _isTick;
        [SerializeField] private MutableValue _tickCycleMutableValue;
        [SerializeField] private float _tickCycle;
        [SerializeField] private MutableValue _tickCountMutableValue;
        [SerializeField] private int _tickCount;
        [SerializeField] private bool _isInfinity;
        [SerializeField] private MutableValue _durationMutableValue;
        [SerializeField] private float _duration;
        [SerializeField] private SkillTypeTemplate _elementalType;

        [SerializeField] private List<ApplyTypeByAmountData> _applyTypeByAmountDatas = new List<ApplyTypeByAmountData>();

        #region MutableValue 처리
        public void Initialize()
        {
            _repeatCountMutableValue = new MutableValue();
            _tickCycleMutableValue = new MutableValue();
            _tickCountMutableValue = new MutableValue();
            _durationMutableValue = new MutableValue();
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

            if (_durationMutableValue.bindKey == bindKey)
            {
                value = _durationMutableValue.GetValueString(_duration, context);
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

            string result = $"{repeatCount}회에 걸쳐";

            if (_isTick)
            {
                float tickCycle = _tickCycleMutableValue.GetPreviewValue(_tickCycle);
                int tickCount = _tickCountMutableValue.GetPreviewValue(_tickCount);
                float tickTime = tickCycle * tickCount;
                result += $", {tickTime}초 동안 {tickCycle}초 마다";
            }

            if (_isInfinity)
            {
                result += " 무한지속";
            }
            else
            {
                float duration = _durationMutableValue.GetPreviewValue(_duration);
                result += $" {duration}초 동안 유지되는";
            }

            return result + " 보호막을 획득합니다.";
        }
        #endregion

        public void Execute(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            int shield = GetAmount(effectContext, casterUnit, targetUnit);

            Execute_RepeatCount(effectContext, casterUnit, targetUnit, shield);
        }

        #region 데미지 계산
        private int GetAmount(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            float totalAmount = 0;

            foreach (var applyTypeByAmountData in _applyTypeByAmountDatas)
            {
                totalAmount += applyTypeByAmountData.GetAmount(effectContext, casterUnit, targetUnit);
            }

            return GetElementEngraveAmount(totalAmount);
        }

        /// <summary>
        /// 속성 진화 적용 값
        /// </summary>
        private int GetElementEngraveAmount(float totalAmount)
        {
            //if (_elementalType != null)
            //{
            //    totalAmount += (totalAmount * GetLevelAmount(_elementalType.engraveLevel));
            //}

            return (int)totalAmount;
        }
        #endregion

        #region 피해 횟수
        private void Execute_RepeatCount(EffectContext context, Unit casterUnit, Unit targetUnit, int shield)
        {
            int repeatCount = _repeatCountMutableValue.GetValue(_repeatCount, context);

            if (repeatCount > 1)
            {
                for (int i = 0; i < repeatCount; i++)
                {
                    if (targetUnit.isDie) return;

                    Execute_Tick(context, casterUnit, targetUnit, shield);
                }
            }
            else
            {
                Execute_Tick(context, casterUnit, targetUnit, shield);
            }
        }
        #endregion

        #region 지속 피해
        private void Execute_Tick(EffectContext context, Unit casterUnit, Unit targetUnit, int shield)
        {
            if (_isTick)
            {
                targetUnit.StartCoroutine(CoExecute_Tick(context, casterUnit, targetUnit, shield));
            }
            else
            {
                Execute_Duration(context, casterUnit, targetUnit, shield);
            }
        }

        private IEnumerator CoExecute_Tick(EffectContext context, Unit casterUnit, Unit targetUnit, int shield)
        {
            var tickCycle = _tickCycleMutableValue.GetValue(_tickCycle, context);
            var tickCount = _tickCountMutableValue.GetValue(_tickCount, context);

            var wfs = new WaitForSeconds(tickCycle);

            for (int i = 0; i < tickCount; i++)
            {
                if (targetUnit.isDie) yield break;

                Execute_Duration(context, casterUnit, targetUnit, shield);
                yield return wfs;
            }
        }
        #endregion

        #region 지속시간 부여
        private void Execute_Duration(EffectContext context, Unit casterUnit, Unit targetUnit, int shield)
        {
            if (_isInfinity)
            {
                targetUnit.healthAbility.AddShield(shield);
            }
            else
            {
                float duration = _durationMutableValue.GetValue(_duration, context);
                targetUnit.healthAbility.AddShield(shield, duration);
            }
        }
        #endregion

        #region 그리기
#if UNITY_EDITOR
        public void Draw(Rect rect)
        {
            #region 피해 횟수
            EffectDrawUtility.DrawBoxedMutableValue(ref rect, _repeatCountMutableValue, "피해 횟수", valueRect =>
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
                EffectDrawUtility.DrawBoxedMutableValue(ref rect, _tickCycleMutableValue, "주기(초)", valueRect =>
                {
                    _tickCycle = EditorGUI.FloatField(valueRect, _tickCycle);
                });
                #endregion

                #region 주기마다 피해 횟수
                rect.y += 5;
                EffectDrawUtility.DrawBoxedMutableValue(ref rect, _tickCountMutableValue, "주기마다 피해 횟수", valueRect =>
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

            #region 지속시간
            EffectDrawUtility.DrawRow(ref rect, "무한지속 여부", valueRect =>
            {
                _isInfinity = EditorGUI.Toggle(valueRect, _isInfinity);
            });

            if (!_isInfinity)
            {
                EffectDrawUtility.DrawBoxedMutableValue(ref rect, _durationMutableValue, "지속 시간", valueRect =>
                {
                    _duration = EditorGUI.FloatField(valueRect, _duration);
                });
            }
            #endregion

            #region 보호막 양 계산
            rect.y += 20;
            EffectDrawUtility.DrawRow(ref rect, "속성", valueRect =>
            {
                _elementalType = (SkillTypeTemplate)EditorGUI.ObjectField(valueRect, _elementalType, typeof(SkillTypeTemplate), false);
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
            float rowNum = 4;

            if (_isTick)
            {
                rowNum += 4;
            }

            if (!_isInfinity)
            {
                rowNum += 1;
            }

            foreach (var amountData in _applyTypeByAmountDatas)
            {
                rowNum += amountData.GetNumRows();
            }
            rowNum += _repeatCountMutableValue.GetNumRows();
            rowNum += _tickCycleMutableValue.GetNumRows();
            rowNum += _tickCountMutableValue.GetNumRows();
            rowNum += _durationMutableValue.GetNumRows();

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