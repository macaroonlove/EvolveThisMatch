using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AbnormalStatusActiveItemEffect : ActiveItemEffect
    {
        [SerializeField] protected bool _isInfinity;
        [SerializeField] protected float _duration;
        [SerializeField] protected AbnormalStatusTemplate _abnormalStatus;

        public override string GetDescription()
        {
            if (_isInfinity)
            {
                return "엑티브 아이템 대상 유닛들에게 무한 지속 상태이상 적용";
            }
            return $"엑티브 아이템 대상 유닛들에게 {_duration}초 간 상태이상 적용";
        }

        public override void Execute(List<Unit> targetUnits)
        {
            foreach (var targetUnit in targetUnits)
            {
                if (targetUnit == null || targetUnit.isDie) continue;

                if (_isInfinity)
                {
                    targetUnit.GetAbility<AbnormalStatusAbility>().ApplyAbnormalStatus(_abnormalStatus, int.MaxValue);
                }
                else
                {
                    targetUnit.GetAbility<AbnormalStatusAbility>().ApplyAbnormalStatus(_abnormalStatus, _duration);
                }
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "무한지속 사용 여부");
            _isInfinity = EditorGUI.Toggle(valueRect, _isInfinity);
            if (!_isInfinity)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "지속시간");
                _duration = EditorGUI.FloatField(valueRect, _duration);
            }

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "상태이상");
            _abnormalStatus = (AbnormalStatusTemplate)EditorGUI.ObjectField(valueRect, _abnormalStatus, typeof(AbnormalStatusTemplate), false);
        }

        public override int GetNumRows()
        {
            int rowNum = 1;

            if (!_isInfinity)
            {
                rowNum++;
            }

            return rowNum;
        }
#endif
    }
}