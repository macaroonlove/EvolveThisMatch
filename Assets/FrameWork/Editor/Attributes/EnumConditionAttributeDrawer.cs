using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EnumConditionAttribute))]
    public class EnumConditionAttributeDrawer : PropertyDrawer
    {
        private static Dictionary<string, string> cachedPaths = new Dictionary<string, string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attributes = fieldInfo.GetCustomAttributes(typeof(EnumConditionAttribute), true).Cast<EnumConditionAttribute>().ToArray();
            bool enabled = attributes.All(attr => GetConditionAttributeResult(attr, property));

            bool previouslyEnabled = GUI.enabled;
            GUI.enabled = enabled;
            if (!attributes[0].Hidden || enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            GUI.enabled = previouslyEnabled;
        }

        private bool GetConditionAttributeResult(EnumConditionAttribute enumConditionAttribute, SerializedProperty property)
        {
            bool enabled = true;

            SerializedProperty enumProp;
            string enumPropPath = string.Empty;
            string propertyPath = property.propertyPath;

            if (!cachedPaths.TryGetValue(propertyPath + enumConditionAttribute.ConditionEnum, out enumPropPath))
            {
                enumPropPath = propertyPath.Replace(property.name, enumConditionAttribute.ConditionEnum);
                cachedPaths[propertyPath + enumConditionAttribute.ConditionEnum] = enumPropPath;
            }

            enumProp = property.serializedObject.FindProperty(enumPropPath);

            if (enumProp != null)
            {
                int currentEnum = enumProp.enumValueIndex;
                enabled = enumConditionAttribute.ContainsBitFlag(currentEnum);
            }
            else
            {
                Debug.LogWarning("enum 타입이 아닙니다. " + enumConditionAttribute.ConditionEnum);
            }

            return enabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            EnumConditionAttribute enumConditionAttribute = (EnumConditionAttribute)attribute;
            bool enabled = GetConditionAttributeResult(enumConditionAttribute, property);

            if (!enumConditionAttribute.Hidden || enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
#endif
}