using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AbnormalStatusSkillEffect : SkillEffect
    {
        [SerializeField] private bool _isInfinity;
        [SerializeField] private float _duration;
        [SerializeField] private AbnormalStatusTemplate _abnormalStatus;

        public override string GetDescription()
        {
            return "상태이상";
        }

        internal override void SkillImpact(Unit casterUnit, Unit targetUnit)
        {
            if (_isInfinity)
            {
                targetUnit.GetAbility<AbnormalStatusAbility>().ApplyAbnormalStatus(_abnormalStatus, int.MaxValue);
            }
            else
            {
                targetUnit.GetAbility<AbnormalStatusAbility>().ApplyAbnormalStatus(_abnormalStatus, _duration);
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
            int rowNum = base.GetNumRows();

            if (!_isInfinity)
            {
                rowNum++;
            }

            return rowNum;
        }
#endif
    }
}