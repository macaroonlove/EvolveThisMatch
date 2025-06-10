using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class BuffSkillEffect : SkillEffect
    {
        [SerializeField] private bool _isInfinity;
        [SerializeField] private float _duration;
        [SerializeField] private BuffTemplate _buff;

        public override string GetDescription()
        {
            return "버프";
        }

        internal override void SkillImpact(Unit casterUnit, Unit targetUnit)
        {
            if (_isInfinity)
            {
                targetUnit.GetAbility<BuffAbility>().ApplyBuff(_buff, int.MaxValue);
            }
            else
            {
                targetUnit.GetAbility<BuffAbility>().ApplyBuff(_buff, _duration);
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
            GUI.Label(labelRect, "버프");
            _buff = (BuffTemplate)EditorGUI.ObjectField(valueRect, _buff, typeof(BuffTemplate), false);            
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