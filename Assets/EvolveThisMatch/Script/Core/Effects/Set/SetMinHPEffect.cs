using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class SetMinHPEffect : Effect
    {
        [SerializeField] private int _value;

        public int value => _value;

        public override string GetDescription()
        {
            if (_value < 0) _value = 0;

            if (_value == 0)
            {
                return $"최소 체력을 설정하세요.";
            }
            else
            {
                return $"최소 체력  {_value}";
            }
        }

#if UNITY_EDITOR
        public override int GetNumRows()
        {
            return 2;
        }

        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 100, rect.height);
            var valueRect = new Rect(rect.x + 100, rect.y, rect.width - 100, rect.height);
            GUI.Label(labelRect, "최소 체력");
            _value = EditorGUI.IntField(valueRect, _value);
        }
#endif
    }
}