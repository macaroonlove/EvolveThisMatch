#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public static class EffectDrawUtility
    {
        public static void DrawRow(ref Rect rect, string label, UnityAction<Rect> drawField, float labelWidth = 140f, float height = 20f, float valueWidthMargin = 0)
        {
            var labelRect = new Rect(rect.x, rect.y, labelWidth, height);
            var valueRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth - valueWidthMargin, height);

            GUI.Label(labelRect, label);
            drawField(valueRect);

            rect.y += height;
        }

        public static void DrawBox(ref Rect rect, string label, UnityAction<Rect> drawField, float labelWidth = 140f, float boxHeight = 20f, float valueWidthMargin = 0)
        {
            Color boxColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.12f) : new Color(0, 0, 0, 0.12f);

            var boxRect = new Rect(rect.x - 8, rect.y - 2, rect.width + 16, boxHeight + 4);
            EditorGUI.DrawRect(boxRect, boxColor);

            DrawRow(ref rect, label, drawField, labelWidth, valueWidthMargin: valueWidthMargin);
        }

        public static void DrawBoxedMutableValue(ref Rect rect, MutableValue mutable, string label, UnityAction<Rect> drawField, float labelWidth = 140f)
        {
            Color boxColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.08f) : new Color(0, 0, 0, 0.08f);

            float startY = rect.y;
            float height = mutable.GetHeight() + 20f;

            var boxRect = new Rect(rect.x - 5, rect.y - 1, rect.width + 10, height + 2);
            EditorGUI.DrawRect(boxRect, boxColor);

            // MutableValue
            mutable.Draw(new Rect(rect.x, rect.y, rect.width, 0));
            rect.y += mutable.GetHeight();

            DrawRow(ref rect, label, drawField, labelWidth);
        }
    }
}
#endif