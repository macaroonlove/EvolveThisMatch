using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Vector2 && property.propertyType != SerializedPropertyType.Vector2Int)
            {
                EditorGUI.LabelField(position, label.text, "Vector2 또는 Vector2Int에 붙여야 합니다.");
                return;
            }

            MinMaxSliderAttribute minMaxSlider = (MinMaxSliderAttribute)attribute;
            float minValue = minMaxSlider.MinValue;
            float maxValue = minMaxSlider.MaxValue;

            float sliderMinValue, sliderMaxValue;
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                Vector2 value = property.vector2Value;
                sliderMinValue = value.x;
                sliderMaxValue = value.y;

                DrawSlider(position, label, ref sliderMinValue, ref sliderMaxValue, minValue, maxValue, true);
                property.vector2Value = new Vector2(sliderMinValue, sliderMaxValue);
            }
            else
            {
                Vector2Int value = property.vector2IntValue;
                sliderMinValue = value.x;
                sliderMaxValue = value.y;

                DrawSlider(position, label, ref sliderMinValue, ref sliderMaxValue, minValue, maxValue, false);
                property.vector2IntValue = new Vector2Int(Mathf.RoundToInt(sliderMinValue), Mathf.RoundToInt(sliderMaxValue));
            }
        }

        private void DrawSlider(Rect position, GUIContent label, ref float minValue, ref float maxValue, float minRange, float maxRange, bool isFloat)
        {
            Rect sliderRect = new Rect(position.x, position.y, position.width - 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(sliderRect, label, ref minValue, ref maxValue, minRange, maxRange);

            if (minValue > maxValue)
            {
                float temp = minValue;
                minValue = maxValue;
                maxValue = temp;
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (isFloat)
                {
                    minValue = Mathf.Clamp(minValue, minRange, maxRange);
                    maxValue = Mathf.Clamp(maxValue, minRange, maxRange);
                }
                else
                {
                    minValue = Mathf.Clamp(Mathf.Round(minValue), Mathf.Round(minRange), Mathf.Round(maxRange));
                    maxValue = Mathf.Clamp(Mathf.Round(maxValue), Mathf.Round(minRange), Mathf.Round(maxRange));
                }
            }

            float inputWidth = 50;
            Rect minInputRect = new Rect(sliderRect.x + sliderRect.width - 105, sliderRect.y + EditorGUIUtility.singleLineHeight + 2, inputWidth, EditorGUIUtility.singleLineHeight);
            Rect maxInputRect = new Rect(sliderRect.x + sliderRect.width - 105 + inputWidth + 5, sliderRect.y + EditorGUIUtility.singleLineHeight + 2, inputWidth, EditorGUIUtility.singleLineHeight);

            if (isFloat)
            {
                float newMin = EditorGUI.FloatField(minInputRect, minValue);
                float newMax = EditorGUI.FloatField(maxInputRect, maxValue);

                if (EditorGUI.EndChangeCheck())
                {
                    if (newMin <= newMax)
                    {
                        minValue = Mathf.Clamp(newMin, minRange, maxRange);
                        maxValue = Mathf.Clamp(newMax, minRange, maxRange);
                    }
                }
            }
            else
            {
                int newMin = EditorGUI.IntField(minInputRect, Mathf.RoundToInt(minValue));
                int newMax = EditorGUI.IntField(maxInputRect, Mathf.RoundToInt(maxValue));

                if (EditorGUI.EndChangeCheck())
                {
                    if (newMin <= newMax)
                    {
                        minValue = Mathf.Clamp(newMin, Mathf.RoundToInt(minRange), Mathf.RoundToInt(maxRange));
                        maxValue = Mathf.Clamp(newMax, Mathf.RoundToInt(minRange), Mathf.RoundToInt(maxRange));
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}
