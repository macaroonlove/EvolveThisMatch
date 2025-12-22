using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class HealEffectLogic
    {
        [SerializeField] private MutableValue _repeatCountMutableValue;
        [SerializeField] private int _repeatCount;
        [SerializeField] private bool _isTick;
        [SerializeField] private MutableValue _tickCycleMutableValue;
        [SerializeField] private int _tickCycle;
        [SerializeField] private MutableValue _tickCountMutableValue;
        [SerializeField] private int _tickCount;
        [SerializeField] private SkillTypeTemplate _elementalType;

        [SerializeField] private List<ApplyTypeByAmountData> _applyTypeByAmountDatas = new List<ApplyTypeByAmountData>();

        public HealEffectLogic()
        {
            _repeatCountMutableValue = new MutableValue();
            _tickCycleMutableValue = new MutableValue();
            _tickCountMutableValue = new MutableValue();
        }

        public void Execute(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            int heal = GetAmount(casterUnit, targetUnit);

            Execute_RepeatCount(casterUnit, targetUnit, heal);
        }

        #region 데미지 계산
        private int GetAmount(Unit casterUnit, Unit targetUnit)
        {
            float totalAmount = 0;

            foreach (var applyTypeByAmountData in _applyTypeByAmountDatas)
            {
                float typeValue = 0f;
                switch (applyTypeByAmountData.applyType)
                {
                    case EApplyType.Basic:
                        typeValue = 1;
                        break;
                    case EApplyType.Caster_FinalATK:
                        typeValue = casterUnit.GetAbility<AttackAbility>().finalATK;
                        break;
                    case EApplyType.Caster_CurrentHP:
                        typeValue = casterUnit.GetAbility<HealthAbility>().currentHP;
                        break;
                    case EApplyType.Caster_MAXHP:
                        typeValue = casterUnit.GetAbility<HealthAbility>().finalMaxHP;
                        break;
                    case EApplyType.Target_CurrentHP:
                        typeValue = targetUnit.GetAbility<HealthAbility>().currentHP;
                        break;
                    case EApplyType.Target_MAXHP:
                        typeValue = targetUnit.GetAbility<HealthAbility>().finalMaxHP;
                        break;
                }

                totalAmount += (typeValue * applyTypeByAmountData.amount);
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
        private void Execute_RepeatCount(Unit casterUnit, Unit targetUnit, int heal)
        {
            if (_repeatCount > 1)
            {
                for (int i = 0; i < _repeatCount; i++)
                {
                    if (targetUnit.isDie) return;

                    Execute_Tick(casterUnit, targetUnit, heal);
                }
            }
            else
            {
                Execute_Tick(casterUnit, targetUnit, heal);
            }
        }
        #endregion

        #region 지속 피해
        private void Execute_Tick(Unit casterUnit, Unit targetUnit, int heal)
        {
            if (_isTick)
            {
                targetUnit.StartCoroutine(CoExecute_Tick(casterUnit, targetUnit, heal));
            }
            else
            {
                targetUnit.healthAbility.Healed(heal, casterUnit);
            }
        }

        private IEnumerator CoExecute_Tick(Unit casterUnit, Unit targetUnit, int heal)
        {
            var wfs = new WaitForSeconds(_tickCycle);

            for (int i = 0; i < _tickCount; i++)
            {
                if (targetUnit.isDie) yield break;

                targetUnit.healthAbility.Healed(heal, casterUnit);
                yield return wfs;
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
                    _tickCycle = EditorGUI.IntField(valueRect, _tickCycle);
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

            #region 회복량 계산
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
            float rowNum = 3;

            if (_isTick)
            {
                rowNum += 4;
            }

            foreach (var amountData in _applyTypeByAmountDatas)
            {
                rowNum += amountData.GetNumRows();
            }
            rowNum += _repeatCountMutableValue.GetNumRows();
            rowNum += _tickCycleMutableValue.GetNumRows();
            rowNum += _tickCountMutableValue.GetNumRows();

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