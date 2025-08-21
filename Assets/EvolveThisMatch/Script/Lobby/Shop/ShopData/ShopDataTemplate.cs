using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [CreateAssetMenu(menuName = "Templates/Lobby/ShopData", fileName = "ShopData", order = 0)]
    public class ShopDataTemplate : ScriptableObject
    {
        [SerializeField] private string _mainTabName;
        [SerializeField] private Sprite _background;
        [SerializeField] private List<ShopData> _shopDatas = new List<ShopData>();

        public string mainTabName => _mainTabName;
        public Sprite background => _background;
        public IReadOnlyList<ShopData> shopDatas => _shopDatas;

#if UNITY_EDITOR
        public void AddShopData(ShopData data) => _shopDatas.Add(data);
#endif
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
        private ShopDataTemplate _target;

        private SerializedProperty _mainTabName;
        private SerializedProperty _background;

        private SerializedProperty _shopDatas;
        private ReorderableList _dataList;

        private void OnEnable()
        {
            _target = target as ShopDataTemplate;

            _mainTabName = serializedObject.FindProperty("_mainTabName");
            _background = serializedObject.FindProperty("_background");

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
            GUILayout.Label("배경", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_background, GUIContent.none);
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
                    var element = _shopDatas.GetArrayElementAtIndex(index).objectReferenceValue as ShopData;

                    rect.x += 10;
                    rect.width -= 10;

                    if (element != null)
                    {
                        rect.y += 2;
                        rect.width -= 10;
                        rect.height = EditorGUIUtility.singleLineHeight;

                        var label = $"{element.subTabName} ({element.GetType().Name})";
                        EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);

                        rect.y += 5;
                        rect.y += EditorGUIUtility.singleLineHeight;

                        element.Draw(rect);

                        if (GUI.changed)
                        {
                            EditorUtility.SetDirty(element);
                        }
                    }
                },
                elementHeightCallback = (index) =>
                {
                    var element = _target.shopDatas[index];
                    return element.GetHeight();
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

        private void CreateDataCallback(object obj)
        {
            var data = ScriptableObject.CreateInstance((Type)obj) as ShopData;

            if (data != null)
            {
                data.hideFlags = HideFlags.HideInHierarchy;
                _target.AddShopData(data);

                var template = target as ShopDataTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(data, path);
                EditorUtility.SetDirty(template);
            }
        }
    }
}