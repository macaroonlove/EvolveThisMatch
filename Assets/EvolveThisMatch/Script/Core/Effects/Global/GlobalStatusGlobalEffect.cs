using ScriptableObjectArchitecture;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class GlobalStatusGlobalEffect : GlobalEffect
    {
        [SerializeField] protected bool _isInfinity;
        [SerializeField] protected float _duration;
        [SerializeField] protected GlobalStatusTemplate _globalStatus;

        public override string GetDescription()
        {
            return "전역 상태 적용";
        }

        public override void Execute()
        {
            if (_isInfinity)
            {
                CoreManager.Instance.GetSubSystem<GlobalStatusSystem>().ApplyGlobalStatus(_globalStatus, int.MaxValue);
            }
            else
            {
                CoreManager.Instance.GetSubSystem<GlobalStatusSystem>().ApplyGlobalStatus(_globalStatus, _duration);
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
            GUI.Label(labelRect, "전역 상태");
            _globalStatus = (GlobalStatusTemplate)EditorGUI.ObjectField(valueRect, _globalStatus, typeof(GlobalStatusTemplate), false);
        }

        public override int GetNumRows()
        {
            int rowNum = 2;

            if (!_isInfinity)
            {
                rowNum++;
            }

            return rowNum;
        }
#endif
    }
}
