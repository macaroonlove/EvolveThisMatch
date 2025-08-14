using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [CreateAssetMenu(menuName = "Templates/Lobby/ShopData", fileName = "ShopData", order = 0)]
    public class ShopDataTemplate : ScriptableObject
    {
        [SerializeField] private string _mainTabName;
        [SerializeField] private Sprite _mainTabIcon;
        [SerializeReference] private List<ShopData> _shopDatas = new List<ShopData>();

        public string mainTabName => _mainTabName;
        public Sprite mainTabIcon => _mainTabIcon;
        public IReadOnlyList<ShopData> shopDatas => _shopDatas;
    }
}

namespace EvolveThisMatch.Editor
{
    using EvolveThisMatch.Lobby;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(ShopDataTemplate)), CanEditMultipleObjects]
    public class ShopDataTemplateEditor : Editor
    {
        private SerializedProperty _mainTabName;
        private SerializedProperty _mainTabIcon;

        private SerializedProperty _shopDatas;
        private ReorderableList _dataList;

        private void OnEnable()
        {
            _mainTabName = serializedObject.FindProperty("_mainTabName");
            _mainTabIcon = serializedObject.FindProperty("_mainTabIcon");

            _shopDatas = serializedObject.FindProperty("_shopDatas");
            CreateDataList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            GUILayout.Label("메인 탭 이름", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_mainTabName, GUIContent.none);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("메인 탭 아이콘", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_mainTabIcon, GUIContent.none);
            GUILayout.EndHorizontal();

            _dataList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateDataList()
        {
            _dataList = new ReorderableList(serializedObject, _shopDatas, true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "상점 데이터");
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = _shopDatas.GetArrayElementAtIndex(index);

                    rect.x += 10;
                    rect.width -= 10;

                    var labelRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    var subTabName = element.FindPropertyRelative("_subTabName").stringValue;
                    var obj = element.managedReferenceValue;
                    var tabName = $"{subTabName} ({obj.GetType().Name})";
                    EditorGUI.LabelField(labelRect, tabName, EditorStyles.boldLabel);

                    EditorGUI.PropertyField(rect, element, GUIContent.none, true);
                },
                elementHeightCallback = (index) =>
                {
                    var element = _shopDatas.GetArrayElementAtIndex(index);
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

                    _shopDatas.DeleteArrayElementAtIndex(list.index);
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                },
            };
        }

        private void InitMenu()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("기본 배치 서브탭"), false, () => CreateDataCallback(typeof(DefaultShopData)));
            menu.AddItem(new GUIContent("랜덤 배치 서브탭"), false, () => CreateDataCallback(typeof(RandomShopData)));

            menu.ShowAsContext();
        }

        private void CreateDataCallback(Type type)
        {
            var instance = Activator.CreateInstance(type) as ShopData;

            if (instance != null)
            {
                _shopDatas.arraySize++;
                var newElement = _shopDatas.GetArrayElementAtIndex(_shopDatas.arraySize - 1);
                newElement.managedReferenceValue = instance;

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}