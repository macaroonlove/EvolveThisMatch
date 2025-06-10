using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class SetDamageTypeEffect : Effect
    {
        [SerializeField] private EDamageType _value;

        public EDamageType value => _value;

        public override string GetDescription()
        {
            return $"피해량 적용 방식을 {_value}으로 변경";
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
            GUI.Label(labelRect, "타입");
            _value = (EDamageType)EditorGUI.EnumPopup(valueRect, _value);
        }
#endif
    }
}