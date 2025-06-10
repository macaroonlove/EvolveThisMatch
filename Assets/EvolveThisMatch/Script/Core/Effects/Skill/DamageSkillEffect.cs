using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class DamageSkillEffect : SkillEffect
    {
        [SerializeField] private int _repeatCount;
        [SerializeField] private bool _isTick;
        [SerializeField] private int _tickCycle;
        [SerializeField] private int _tickCount;
        [SerializeField] private EDamageType _damageType;

        [SerializeField] private List<ApplyTypeByAmountData> _applyTypeByAmountDatas = new List<ApplyTypeByAmountData>();

        public override string GetDescription()
        {
            return "데미지";
        }

        public int GetAmount(Unit casterUnit, Unit targetUnit)
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
                    case EApplyType.ATK:
                        typeValue = casterUnit.GetAbility<AttackAbility>().baseATK;
                        break;
                    case EApplyType.FinalATK:
                        typeValue = casterUnit.GetAbility<AttackAbility>().finalATK;
                        break;
                    case EApplyType.CurrentHP:
                        typeValue = casterUnit.GetAbility<HealthAbility>().currentHP;
                        break;
                    case EApplyType.MAXHP:
                        typeValue = casterUnit.GetAbility<HealthAbility>().finalMaxHP;
                        break;
                    case EApplyType.Enemy_CurrentHP:
                        typeValue = targetUnit.GetAbility<HealthAbility>().currentHP;
                        break;
                    case EApplyType.Enemy_MAXHP:
                        typeValue = targetUnit.GetAbility<HealthAbility>().finalMaxHP;
                        break;
                }

                totalAmount += typeValue * applyTypeByAmountData.amount;
            }

            return (int)totalAmount;
        }

        internal override void SkillImpact(Unit casterUnit, Unit targetUnit)
        {
            int damage = GetAmount(casterUnit, targetUnit);

            Execute_RepeatCount(casterUnit, targetUnit, damage);
        }

        private void Execute_RepeatCount(Unit casterUnit, Unit targetUnit, int damage)
        {
            if (_repeatCount > 1)
            {
                for (int i = 0; i < _repeatCount; i++)
                {
                    if (targetUnit.isDie) return;

                    Execute_Tick(casterUnit, targetUnit, damage);
                }
            }
            else
            {
                Execute_Tick(casterUnit, targetUnit, damage);
            }
        }        

        private void Execute_Tick(Unit casterUnit, Unit targetUnit, int damage)
        {
            if (_isTick)
            {
                targetUnit.StartCoroutine(CoExecute_Tick(casterUnit, targetUnit, damage));
            }
            else
            {
                Execute_DamageType(casterUnit, targetUnit, damage);
            }
        }

        private IEnumerator CoExecute_Tick(Unit casterUnit, Unit targetUnit, int damage)
        {
            var wfs = new WaitForSeconds(_tickCycle);

            for (int i = 0; i < _tickCount; i++)
            {
                if (targetUnit.isDie) yield break;

                Execute_DamageType(casterUnit, targetUnit, damage);
                yield return wfs;
            }
        }

        private void Execute_DamageType(Unit casterUnit, Unit targetUnit, int damage)
        {
            if (_damageType == EDamageType.TrueDamage)
            {
                targetUnit.GetAbility<HitAbility>().Hit(damage, casterUnit.id);
            }
            else
            {
                targetUnit.GetAbility<HitAbility>().Hit(damage, _damageType, casterUnit.id);
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
                _applyTypeByAmountDatas.Add(new ApplyTypeByAmountData());
            }

            var half = (rect.width - 24) * 0.5f;
            var applyTypeRect = new Rect(labelRect.x, labelRect.y, half, 20);
            var amountRect = new Rect(half + 44, labelRect.y, half, 20);
            var deleteRect = new Rect(rect.width + 22, valueRect.y, 20, 20);

            for (int i = 0; i < _applyTypeByAmountDatas.Count; i++)
            {
                var data = _applyTypeByAmountDatas[i];

                applyTypeRect.y += 20;
                amountRect.y += 20;
                deleteRect.y += 20;

                data.applyType = (EApplyType)EditorGUI.EnumPopup(applyTypeRect, data.applyType);
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
            int rowNum = base.GetNumRows();

            rowNum += 4;

            if (_isTick)
            {
                rowNum += 2;
            }

            rowNum += _applyTypeByAmountDatas.Count;

            return rowNum;
        }
#endif
    }
}