using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelAttribute labelAttribute = (LabelAttribute)attribute;

            // 인스펙터에 표기될 이름 바꾸기
            label.text = labelAttribute.label;

            // 프로퍼티 그리기
            if (property.propertyType == SerializedPropertyType.String)
            {
                // Multiline 처리
                MultilineAttribute multiline = (MultilineAttribute)fieldInfo.GetCustomAttribute(typeof(MultilineAttribute));
                if (multiline != null)
                {
                    position.height = EditorGUIUtility.singleLineHeight * multiline.lines;
                }

                // TextArea 처리
                TextAreaAttribute textArea = (TextAreaAttribute)fieldInfo.GetCustomAttribute(typeof(TextAreaAttribute));
                if (textArea != null)
                {
                    EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);

                    position.y += EditorGUIUtility.singleLineHeight + 5;
                    position.height = EditorGUIUtility.singleLineHeight * Mathf.Max(textArea.minLines, textArea.maxLines);
                    property.stringValue = EditorGUI.TextArea(position, property.stringValue, EditorStyles.textArea);
                    return;
                }
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Multiline 처리
            MultilineAttribute multiline = (MultilineAttribute)fieldInfo.GetCustomAttribute(typeof(MultilineAttribute));
            if (multiline != null)
            {
                return EditorGUIUtility.singleLineHeight * multiline.lines;
            }

            // TextArea 처리
            TextAreaAttribute textArea = (TextAreaAttribute)fieldInfo.GetCustomAttribute(typeof(TextAreaAttribute));
            if (textArea != null)
            {
                return EditorGUIUtility.singleLineHeight * textArea.maxLines + EditorGUIUtility.singleLineHeight * 2;
            }

            return base.GetPropertyHeight(property, label);
        }
    }
#endif
}