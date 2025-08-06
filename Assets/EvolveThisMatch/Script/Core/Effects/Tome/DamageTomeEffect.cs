using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class DamageTomeEffect : TomeEffect
    {
        [SerializeField] protected int _repeatCount;
        [SerializeField] protected bool _isTick;
        [SerializeField] protected int _tickCycle;
        [SerializeField] protected int _tickCount;
        [SerializeField] protected EDamageType _damageType;

        [SerializeField] protected List<ApplyType_TargetOnlyByAmountData> _applyTypeByAmountDatas = new List<ApplyType_TargetOnlyByAmountData>();

        public override string GetDescription()
        {
            return "고서 대상 유닛들에게 데미지 적용";
        }

        public int GetAmount(Unit targetUnit, int level)
        {
            float totalAmount = 0;

            foreach (var applyTypeByAmountData in _applyTypeByAmountDatas)
            {
                float typeValue = 0f;
                switch (applyTypeByAmountData.applyType)
                {
                    case EApplyType_TargetOnly.Basic:
                        typeValue = 1;
                        break;
                    case EApplyType_TargetOnly.Enemy_CurrentHP:
                        typeValue = targetUnit.GetAbility<HealthAbility>().currentHP;
                        break;
                    case EApplyType_TargetOnly.Enemy_MAXHP:
                        typeValue = targetUnit.GetAbility<HealthAbility>().finalMaxHP;
                        break;
                }

                // TODO: 일단은 모든 데미지 방식에 레벨 만큼 보정, 추후 수정
                totalAmount += typeValue * (applyTypeByAmountData.amount + level);
            }

            return (int)totalAmount;
        }

        public override void Execute(List<Unit> targetUnits, int level)
        {
            foreach (var targetUnit in targetUnits)
            {
                if (targetUnit == null || targetUnit.isDie) continue;

                int damage = GetAmount(targetUnit, level);

                Execute_RepeatCount(targetUnit, damage);
            }
        }

        private void Execute_RepeatCount(Unit targetUnit, int damage)
        {
            if (_repeatCount > 1)
            {
                for (int i = 0; i < _repeatCount; i++)
                {
                    if (targetUnit.isDie) return;

                    Execute_Tick(targetUnit, damage);
                }
            }
            else
            {
                Execute_Tick(targetUnit, damage);
            }
        }

        private void Execute_Tick(Unit targetUnit, int damage)
        {
            if (_isTick)
            {
                targetUnit.StartCoroutine(CoExecute_Tick(targetUnit, damage));
            }
            else
            {
                Execute_DamageType(targetUnit, damage);
            }
        }

        private IEnumerator CoExecute_Tick(Unit targetUnit, int damage)
        {
            var wfs = new WaitForSeconds(_tickCycle);

            for (int i = 0; i < _tickCount; i++)
            {
                if (targetUnit.isDie) yield break;

                Execute_DamageType(targetUnit, damage);
                yield return wfs;
            }
        }

        private void Execute_DamageType(Unit targetUnit, int damage)
        {
            if (_damageType == EDamageType.TrueDamage)
            {
                targetUnit.GetAbility<HitAbility>().Hit(damage);
            }
            else
            {
                targetUnit.GetAbility<HitAbility>().Hit(damage, _damageType);
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "피해 횟수");
            _repeatCount = EditorGUI.IntField(valueRect, _repeatCount);
            if (_repeatCount <= 0) _repeatCount = 1;

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "주기마다 피해 사용 여부");
            _isTick = EditorGUI.Toggle(valueRect, _isTick);
            if (_isTick)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "주기(초)");
                _tickCycle = EditorGUI.IntField(valueRect, _tickCycle);

                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "주기마다 피해 횟수");
                _tickCount = EditorGUI.IntField(valueRect, _tickCount);
            }

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "데미지 타입");
            _damageType = (EDamageType)EditorGUI.EnumPopup(valueRect, _damageType);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "적용 방식");
            if (GUI.Button(valueRect, "추가"))
            {
                _applyTypeByAmountDatas.Add(new ApplyType_TargetOnlyByAmountData());
            }

            var half = (rect.width - 24) * 0.5f;
            var applyTypeRect = new Rect(labelRect.x, labelRect.y, half, 20);
            var amountRect = new Rect(half + 24, labelRect.y, half, 20);
            var deleteRect = new Rect(rect.width, valueRect.y, 20, 20);

            for (int i = 0; i < _applyTypeByAmountDatas.Count; i++)
            {
                var data = _applyTypeByAmountDatas[i];

                applyTypeRect.y += 20;
                amountRect.y += 20;
                deleteRect.y += 20;

                data.applyType = (EApplyType_TargetOnly)EditorGUI.EnumPopup(applyTypeRect, data.applyType);
                data.amount = EditorGUI.FloatField(amountRect, data.amount);

                if (GUI.Button(deleteRect, "X"))
                {
                    _applyTypeByAmountDatas.RemoveAt(i);
                    break;
                }
            }
        }

        public override int GetNumRows()
        {
            int rowNum = 6;

            if (_isTick)
            {
                rowNum += 2;
            }

            rowNum += (int)(_applyTypeByAmountDatas.Count * 1.2f);

            return rowNum;
        }
#endif
    }
}