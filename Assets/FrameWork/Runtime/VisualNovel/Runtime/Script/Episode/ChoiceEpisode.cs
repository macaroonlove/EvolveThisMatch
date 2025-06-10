using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class ChoiceTemplate
    {
        public string choice;
        public string nextChapter;

        public ChoiceTemplate(string choice, string nextChapter)
        {
            this.choice = choice;
            this.nextChapter = nextChapter;
        }
    }

    [Serializable]
    public class ChoiceEpisode : Episode
    {
        public override CommandType command => CommandType.Choice;

        public List<ChoiceTemplate> choiceList = new List<ChoiceTemplate>();

        public void Initialize(string context)
        {
            context = context.Trim('"');
            var options = context.Split('`');

            choiceList.Clear();
            foreach (var option in options)
            {
                var template = option.Split('&');
                var choice = template[0].TrimStart('\n');
                var nextChapter = template[1].TrimEnd('\n');

                choiceList.Add(new ChoiceTemplate(choice, nextChapter));
            }
        }

#if UNITY_EDITOR
        private ReorderableList _reorderableList;
        private SerializedObject _serializedObject;
        private SerializedProperty _choiceProperty;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _choiceProperty = _serializedObject.FindProperty("choiceList");

            _reorderableList = new ReorderableList(_serializedObject, _choiceProperty, true, true, true, true)
            {
                drawHeaderCallback = (Rect headerRect) =>
                {
                    EditorGUI.LabelField(headerRect, "¼±ÅÃÁö");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = _choiceProperty.GetArrayElementAtIndex(index);
                    var choiceProperty = element.FindPropertyRelative("choice");
                    var nextChapterProperty = element.FindPropertyRelative("nextChapter");

                    float choiceWidth = rect.width * 0.7f - 5;
                    var elementRect = new Rect(rect.x, rect.y + 2, choiceWidth, 58);

                    EditorGUI.PropertyField(elementRect, choiceProperty, GUIContent.none);

                    elementRect.x += choiceWidth + 5;
                    elementRect.width = rect.width * 0.3f - 5;
                    elementRect.height = 20;

                    EditorGUI.PropertyField(elementRect, nextChapterProperty, GUIContent.none);
                },
                elementHeightCallback = (index) =>
                {
                    return 60;
                }
            };
        }

        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            if (_serializedObject == null || _choiceProperty == null)
            {
                OnEnable();
            }

            var listRect = new Rect(rect.x + 130, rect.y + 4, rect.width - 130, 18);

            _serializedObject.Update();
            _reorderableList.DoList(listRect);
            _serializedObject.ApplyModifiedProperties();
        }

        public override int GetHeight()
        {
            int height = 4;

            height += choiceList.Count * 3;

            return height;
        }
#endif
    }
}