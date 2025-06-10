using ScriptableObjectArchitecture;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class GlobalEventGameTrigger : GameTrigger
    {
        [SerializeField] private GlobalEvent _globalEvent;

        internal GlobalEvent globalEvent => _globalEvent;

        public override string GetLabel()
        {
            if (_globalEvent != null)
            {
                return $"{_globalEvent.name} 호출 시";
            }
            else
            {
                return "특정 글로벌 이벤트 발생 시";
            }
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            GUILayout.BeginVertical("GroupBox");
            {
                _globalEvent = (GlobalEvent)EditorGUILayout.ObjectField(_globalEvent, typeof(GlobalEvent), false);
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