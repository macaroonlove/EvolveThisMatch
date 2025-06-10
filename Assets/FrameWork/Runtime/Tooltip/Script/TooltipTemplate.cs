using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace FrameWork.Tooltip
{
    [CreateAssetMenu(menuName = "Templates/Tooltip", fileName = "TooltipData", order = 0)]
    public class TooltipTemplate : ScriptableObject
    {
        public List<LinkTooltip> linkTooptips = new List<LinkTooltip>();
    }

    [Serializable]
    public class LinkTooltip
    {
        public string linkName;
        public TooltipStyle style;
        public TooltipData data;
        public string prevStyleName;
    }
}

#if UNITY_EDITOR
namespace FrameWork.Tooltip.Editor
{
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using FrameWork.Tooltip;
    using System.Linq;

    [CustomEditor(typeof(TooltipTemplate))]
    public class TooltipTemplateEditor : Editor
    {
        private ReorderableList _reorderableList;
        private TooltipTemplate _template;
        private LinkTooltip _currentLinkTooltip;

        private void OnEnable()
        {
            _template = (TooltipTemplate)target;

            _reorderableList = new ReorderableList(_template.linkTooptips, typeof(LinkTooltip), true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Link 태그와 연동될 툴팁들");
                },
                onSelectCallback = (list) =>
                {
                    int index = list.index;
                    _currentLinkTooltip = _template.linkTooptips[index];
                },
                onAddCallback = (list) =>
                {
                    var newTooltip = new LinkTooltip();
                    _template.linkTooptips.Add(newTooltip);
                    list.index = _template.linkTooptips.Count - 1;
                    _currentLinkTooltip = newTooltip;
                },
                onRemoveCallback = (list) =>
                {
                    _template.linkTooptips.RemoveAt(list.index);
                    list.index = -1;
                    _currentLinkTooltip = null;
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = _template.linkTooptips[index];

                    rect.y += 2;
                    rect.width -= 10;
                    rect.height = EditorGUIUtility.singleLineHeight;

                    var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
                    var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

                    GUI.Label(labelRect, "링크 이름");
                    element.linkName = EditorGUI.TextField(valueRect, element.linkName);

                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "툴팁 스타일");
                    element.style = (TooltipStyle)EditorGUI.ObjectField(valueRect, element.style, typeof(TooltipStyle), false);
                },
                elementHeightCallback = (int index) =>
                {
                    return 40;
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            _reorderableList.DoLayoutList();

            if (_currentLinkTooltip != null && _currentLinkTooltip.style != null)
            {
                if (_currentLinkTooltip.prevStyleName != _currentLinkTooltip.style.name)
                {
                    _currentLinkTooltip.data = _currentLinkTooltip.style.CreateField();

                    _currentLinkTooltip.prevStyleName = _currentLinkTooltip.style.name;
                }

                // 툴팁 필드 생성
                if (_currentLinkTooltip.data.IsInitialize() == false)
                {
                    _currentLinkTooltip.data = _currentLinkTooltip.style.CreateField();
                }

                // 툴팁 데이터 불러오기
                if (_currentLinkTooltip.data.IsInitializeData() == false)
                {
                    _currentLinkTooltip.data.InitializeData();
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("툴팁 데이터 설정", EditorStyles.boldLabel);

                // 문자열 데이터 처리
                var stringData = _currentLinkTooltip.data.getAllString;
                foreach (var key in stringData.Keys.ToList())
                {
                    EditorGUILayout.LabelField(key);
                    string newValue = EditorGUILayout.TextArea(
                        stringData[key],
                        GUILayout.Height(50),
                        GUILayout.ExpandHeight(true)
                    );
                    if (newValue != stringData[key])
                    {
                        _currentLinkTooltip.data.SetStringEditor(key, newValue);
                        GUI.changed = true;
                    }
                }

                // 스프라이트 데이터 처리
                var spriteData = _currentLinkTooltip.data.getAllSprite;
                foreach (var key in spriteData.Keys.ToList())
                {
                    Sprite newValue = (Sprite)EditorGUILayout.ObjectField(key, spriteData[key], typeof(Sprite), false);
                    if (newValue != spriteData[key])
                    {
                        _currentLinkTooltip.data.SetSpriteEditor(key, newValue);
                        GUI.changed = true;
                    }
                }

                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                if (_currentLinkTooltip?.data != null) _currentLinkTooltip.data.InitializeData();
                EditorUtility.SetDirty(_template);
            }
        }
    }
}
#endif