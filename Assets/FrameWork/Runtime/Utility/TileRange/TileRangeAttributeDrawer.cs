using UnityEditor;
using UnityEngine;

namespace FrameWork
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TileRangeAttribute))]
    public class TileRangeAttributeDrawer : PropertyDrawer
    {
        TileRangeAttribute attackRangeAttribute { get { return ((TileRangeAttribute)attribute); } }

        private const int TileSize = 20;

        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.PropertyField(rect, prop, label, true);

            if (prop.objectReferenceValue == null) return;
            TileRangeTemplate template = prop.objectReferenceValue as TileRangeTemplate;

            rect.y += EditorGUI.GetPropertyHeight(prop);

            var centerX = rect.x + rect.width / 2;

            var originHeightCount = template.GetHeight();
            var heightCount = originHeightCount;
            if (heightCount == 0) heightCount = 1; else heightCount += 2;

            template.GetMinMaxWidth(out var MinX, out var MaxX);
            if (originHeightCount != 0)
            {
                MinX -= 1;
                MaxX += 1;
            }
            var XCount = MaxX - MinX + 1;
            var StartX = centerX - (TileSize * XCount * 0.5f);

            var defaltColor = GUI.color;

            var halfY = heightCount / 2;

            rect.y += TileSize * 0.5f;
            for (int y = 0; y < heightCount; y++)
            {
                var currY = y - halfY;

                for (int x = MinX; x <= MaxX; x++)
                {
                    var Xindex = x - MinX;
                    var RectX = StartX + (TileSize * Xindex);

                    if (y == heightCount / 2 && x == 0)
                    {
                        if (originHeightCount == 0)
                        {
                            GUI.color = defaltColor;
                            if (GUI.Button(new Rect(RectX, rect.y, TileSize, TileSize), ""))
                            {
                                template.Add(0, 0);
                                EditorUtility.SetDirty(template);
                            }
                        }
                        else
                        {
                            GUI.color = Color.blue;
                            if (GUI.Button(new Rect(RectX, rect.y, TileSize, TileSize), ""))
                            {
                                //Do Nothing
                            }
                        }

                    }
                    else if (y == 0 || y == heightCount - 1)
                    {
                        GUI.color = defaltColor;

                        if (GUI.Button(new Rect(RectX, rect.y, TileSize, TileSize), ""))
                        {
                            template.Add(x, currY);
                            EditorUtility.SetDirty(template);
                        }
                    }
                    else
                    {
                        if (template.IsContains(x, currY))
                        {
                            GUI.color = Color.red;
                            if (GUI.Button(new Rect(RectX, rect.y, TileSize, TileSize), ""))
                            {
                                template.Remove(x, currY);
                                EditorUtility.SetDirty(template);
                            }
                        }
                        else
                        {
                            GUI.color = defaltColor;
                            if (GUI.Button(new Rect(RectX, rect.y, TileSize, TileSize), ""))
                            {
                                template.Add(x, currY);
                                EditorUtility.SetDirty(template);
                            }
                        }
                    }
                }
                rect.y += TileSize;
            }
            GUI.color = defaltColor;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null) return EditorGUI.GetPropertyHeight(property);

            TileRangeTemplate template = property.objectReferenceValue as TileRangeTemplate;
            var height = template.GetHeight();
            if (height == 0) height = 1; else height += 2;
            return EditorGUI.GetPropertyHeight(property) + (height * TileSize) + EditorGUIUtility.singleLineHeight;
        }
    }
#endif
}