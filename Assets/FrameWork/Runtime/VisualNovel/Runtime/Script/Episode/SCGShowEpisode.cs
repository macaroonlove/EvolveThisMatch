using System;
using UnityEditor;
using UnityEngine;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class SCGShowEpisode : ThemeEpisode
    {
        public override CommandType command => CommandType.SCG_Show;

        public int id;
        public Rect position;
        public Vector2 anchor;

        public void Initialize(string theme, string cell)
        {
            base.Initialize(theme);

            var contents = cell.Split(' ');
            foreach (var content in contents)
            {
                var keyValue = content.Split(':');

                if (keyValue.Length >= 2)
                {
                    var key = keyValue[0];
                    var value = keyValue[1];

                    switch (key)
                    {
                        case "id":
                            int.TryParse(value, out id);
                            break;
                        case "posX":
                            if (int.TryParse(value, out int x))
                            {
                                position.x = x;
                            }
                            break;
                        case "posY":
                            if (int.TryParse(value, out int y))
                            {
                                position.x = y;
                            }
                            break;
                        case "width":
                            if (int.TryParse(value, out int width))
                            {
                                position.width = width;
                            }
                            break;
                        case "height":
                            if (int.TryParse(value, out int height))
                            {
                                position.height = height;
                            }
                            break;
                        case "horizontal":
                            switch (value)
                            {
                                case "left":
                                    anchor.x = 0;
                                    break;
                                case "center":
                                    anchor.x = 0.5f;
                                    break;
                                case "right":
                                    anchor.x = 1;
                                    break;
                            }
                            break;
                        case "vertical":
                            switch (value)
                            {
                                case "bottom":
                                    anchor.y = 0;
                                    break;
                                case "middle":
                                    anchor.y = 0.5f;
                                    break;
                                case "top":
                                    anchor.y = 1;
                                    break;
                            }
                            break;
                    }
                }
            }
        }

#if UNITY_EDITOR
        private string[] _horizontal = { "Left", "Center", "Right" };
        private string[] _vertical = { "Bottom", "Middle", "Top" };
        private float[] _anchorValues = { 0, 0.5f, 1 };

        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            EditorGUI.LabelField(new Rect(rect.x + 130, rect.y + 24, 60, 18), "SCG ID");
            id = EditorGUI.IntField(new Rect(rect.x + 190, rect.y + 24, 130, 18), id);

            int middle = (int)(rect.width - 330) / 2;
            var labelRect = new Rect(rect.x + 330, rect.y + 4, rect.width - 330, 40);
            var valueRect = new Rect(rect.x + 390, rect.y + 4, middle - 62, 18);

            position = EditorGUI.RectField(labelRect, position);
            labelRect.y += 40;
            valueRect.y += 40;

            labelRect.width = 60;
            labelRect.height = 18;

            EditorGUI.LabelField(labelRect, "¾ÞÄ¿ X");
            int selectedX = Array.IndexOf(_anchorValues, anchor.x);
            selectedX = EditorGUI.Popup(valueRect, selectedX, _horizontal);
            if (selectedX >= 0) anchor.x = _anchorValues[selectedX];

            labelRect.x += middle;
            valueRect.x += middle;
            valueRect.width = middle - 60;
            EditorGUI.LabelField(labelRect, "¾ÞÄ¿ Y");
            int selectedY = Array.IndexOf(_anchorValues, anchor.y);
            selectedY = EditorGUI.Popup(valueRect, selectedY, _vertical);
            if (selectedY >= 0) anchor.y = _anchorValues[selectedY];
        }

        public override int GetHeight()
        {
            return 3;
        }
#endif
    }
}