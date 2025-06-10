using ScriptableObjectArchitecture;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UnitEventGameTrigger : GameTrigger
    {
        [SerializeField] private UnitEvent _unitEvent;

        internal UnitEvent unitEvent => _unitEvent;

        public override string GetLabel()
        {
            if (_unitEvent != null)
            {
                return $"{_unitEvent.name} 호출 시";
            }
            else
            {
                return "특정 유닛 이벤트 발생 시";
            }
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            GUILayout.Space(5);

            GUILayout.BeginVertical("GroupBox");
            {
                _unitEvent = (UnitEvent)EditorGUILayout.ObjectField(_unitEvent, typeof(UnitEvent), false);
            }
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginVertical();
            {
                base.Draw();
            }
            GUILayout.EndVertical();
        }
#endif
    }
}