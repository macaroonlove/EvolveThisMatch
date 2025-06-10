using UnityEngine;
using XNode;

namespace EvolveThisMatch.Core
{
    public class SkillNode : Node
    {
        public int index;
        public SkillTemplate skillTemplate;
        public int startLevel;
        public int minLevel;
        public int maxLevel;

        [Output] public int output;
        [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Multiple)] public int input;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}


#if UNITY_EDITOR
namespace EvolveThisMatch.Editor
{
    using EvolveThisMatch.Core;
    using UnityEngine;
    using XNodeEditor;

    [CustomNodeEditor(typeof(SkillNode))]
    public class SkillNodeEditor : NodeEditor
    {
        public override void OnHeaderGUI()
        {
            var node = target as SkillNode;

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Space(10);

            if (node.skillTemplate != null)
            {
                GUILayout.Label(node.skillTemplate.displayName, style);
            }
            else
            {
                GUILayout.Label("Skill 미등록", style);
            }
        }

        public override void OnBodyGUI()
        {
            var node = target as SkillNode;

            GUILayout.Space(5);
            if (node.skillTemplate != null && node.skillTemplate.sprite != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(node.skillTemplate.sprite.texture, GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("", GUILayout.Width(64), GUILayout.Height(64));
            }

            Rect rect = GUILayoutUtility.GetRect(0, -5);
            float centerX = rect.width / 2f + 10;

            NodeEditorGUILayout.PortField(new Vector2(centerX, 0), node.GetInputPort("input"));
            NodeEditorGUILayout.PortField(new Vector2(centerX, 100), node.GetOutputPort("output"));
        }

        public override int GetWidth()
        {
            return 100;
        }
    }
}
#endif