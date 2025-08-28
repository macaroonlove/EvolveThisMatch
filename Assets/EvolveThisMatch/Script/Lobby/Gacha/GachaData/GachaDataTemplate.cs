using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [CreateAssetMenu(menuName = "Templates/Data/GachaData", fileName = "GachaData", order = 0)]
    public class GachaDataTemplate : ScriptableObject
    {
        [SerializeReference] private List<GachaData> _gachaDatas = new List<GachaData>();

        public IReadOnlyList<GachaData> gachaDatas => _gachaDatas;
    }
}

#if UNITY_EDITOR
namespace EvolveThisMatch.Editor
{
    using EvolveThisMatch.Lobby;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(GachaDataTemplate)), CanEditMultipleObjects]
    public class GachaDataTemplateEditor : Editor
    {
        private SerializedProperty _gachaDatas;
        private ReorderableList _dataList;

        private void OnEnable()
        {
            _gachaDatas = serializedObject.FindProperty("_gachaDatas");
            CreateDataList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _dataList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateDataList()
        {
            _dataList = new ReorderableList(serializedObject, _gachaDatas, true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "가챠 데이터");
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = _gachaDatas.GetArrayElementAtIndex(index);

                    rect.x += 10;
                    rect.width -= 10;

                    var labelRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    var tabName = element.FindPropertyRelative("_tabName").stringValue;
                    EditorGUI.LabelField(labelRect, tabName, EditorStyles.boldLabel);

                    EditorGUI.PropertyField(rect, element, GUIContent.none, true);
                },
                elementHeightCallback = (index) =>
                {
                    var element = _gachaDatas.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(element, true) + 4;
                },
                onAddDropdownCallback = (buttonRect, list) =>
                {
                    InitMenu();
                },
                onRemoveCallback = (list) =>
                {
                    if (!EditorUtility.DisplayDialog("경고!", "이 데이터를 삭제하시겠습니까?", "네", "아니요"))
                        return;

                    _gachaDatas.DeleteArrayElementAtIndex(list.index);
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                },
            };
        }

        private void InitMenu()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("아군 유닛 탭"), false, () => CreateDataCallback(typeof(AgentGachaData)));
            menu.AddItem(new GUIContent("아군 스킨 탭"), false, () => CreateDataCallback(typeof(AgentSkinGachaData)));

            menu.ShowAsContext();
        }

        private void CreateDataCallback(Type type)
        {
            var instance = Activator.CreateInstance(type) as GachaData;

            if (instance != null)
            {
                _gachaDatas.arraySize++;
                var newElement = _gachaDatas.GetArrayElementAtIndex(_gachaDatas.arraySize - 1);
                newElement.managedReferenceValue = instance;

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif