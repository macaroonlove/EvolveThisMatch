using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TextColorUsageAttribute))]
    public class TextColorUsageAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TextColorUsageAttribute textColorUsageAttribute = (TextColorUsageAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            // 원하는 위치로 라벨을 이동
            Rect labelPosition = new Rect(position.x, position.y, 168f, position.height);

            // 라벨의 색을 변경하여 렌더링
            Color originalContentColor = GUI.contentColor;
            GUI.contentColor = textColorUsageAttribute.color;
            EditorGUI.LabelField(labelPosition, label);
            GUI.contentColor = originalContentColor;

            // 프로퍼티 영역을 라벨 이후로 이동
            Rect propertyPosition = new Rect(position.x + 168f, position.y, position.width - 168f, position.height);

            // 프로퍼티를 그림
            EditorGUI.PropertyField(propertyPosition, property, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
#endif
}