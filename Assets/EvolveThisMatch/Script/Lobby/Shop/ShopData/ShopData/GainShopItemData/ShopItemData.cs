using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class ShopItemData
    {
        [SerializeField] private string _itemName;
        [SerializeField] private Sprite _itemIcon;

        [SerializeField] private bool _isCash;
        [SerializeField] private ObscuredIntVariable _variable;
        [SerializeField] private int _needCount;

        [SerializeField] private int _buyAbleCount;
        [SerializeField] private bool _isPackage;
        [SerializeField] private bool _isPanel;

        [SerializeReference] private List<GainShopItemData> _gainShopItemDatas = new List<GainShopItemData>();

        #region 프로퍼티
        public string itemName => _itemName;
        public Sprite itemIcon => _itemIcon;
        public bool isCash => _isCash;
        public ObscuredIntVariable variable => _variable;
        public int needCount => _needCount;
        public int buyAbleCount => _buyAbleCount;
        public bool isPackage => _isPackage;
        public bool isPanel => _isPanel;

        public IReadOnlyList<GainShopItemData> gainShopItemDatas => _gainShopItemDatas;
        #endregion
    }
}

namespace EvolveThisMatch.Editor
{
    using EvolveThisMatch.Lobby;
    using System;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ShopItemData), true)]
    public class ShopItemDataDrawer : PropertyDrawer
    {
        private readonly Dictionary<string, ReorderableList> _lists = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var gainList = GetOrCreateDataList(property);

            EditorGUI.BeginProperty(position, label, property);

            var propertyRect = new Rect(EditorGUIUtility.labelWidth * 0.5f + 45f, position.y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("_itemName"), new GUIContent("아이템 이름"));
            propertyRect.y += 20;

            EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("_itemIcon"), new GUIContent("아이템 이미지"));
            propertyRect.y += 20;
            
            EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("_isCash"), new GUIContent("현금 결제 여부"));
            propertyRect.y += 20;

            if (!property.FindPropertyRelative("_isCash").boolValue)
            {
                EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("_variable"), new GUIContent("필요 재화 종류"));
                propertyRect.y += 20;
            }

            EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("_needCount"), new GUIContent("필요 재화 수"));
            propertyRect.y += 20;

            EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("_buyAbleCount"), new GUIContent("구매 가능 횟수"));
            propertyRect.y += 30;

            EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("_isPackage"), new GUIContent("패키지인가?"));
            propertyRect.y += 20;
            EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("_isPanel"), new GUIContent("패널을 열 것인가?"));
            propertyRect.y += 30;

            propertyRect.height = gainList.GetHeight();
            gainList.DoList(propertyRect);

            EditorGUI.EndProperty();
        }

        private ReorderableList GetOrCreateDataList(SerializedProperty property)
        {
            if (_lists.TryGetValue(property.propertyPath, out var list)) return list;

            var gainDatas = property.FindPropertyRelative("_gainShopItemDatas");
            list = new ReorderableList(property.serializedObject, gainDatas, true, true, true, true)
            {
                drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "획득할 아이템들"),
                drawElementCallback = (rect, index, active, focused) =>
                {
                    var element = gainDatas.GetArrayElementAtIndex(index);
                    var obj = element.managedReferenceValue;

                    // 타입 이름 출력
                    var labelRect = new Rect(rect.x + 10, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(labelRect, obj.GetType().Name, EditorStyles.boldLabel);

                    // 필드 출력
                    var fieldRect = new Rect(rect.x, rect.y, rect.width, rect.height - EditorGUIUtility.singleLineHeight - 2);
                    EditorGUI.PropertyField(fieldRect, element, GUIContent.none, true);
                },
                elementHeightCallback = (index) =>
                {
                    var element = gainDatas.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(element, true) + 2f;
                },
                onAddDropdownCallback = (buttonRect, list) =>
                {
                    InitMenu(gainDatas);
                },
                onRemoveCallback = (list) =>
                {
                    if (!EditorUtility.DisplayDialog("경고!", "이 데이터를 삭제하시겠습니까?", "네", "아니요"))
                        return;

                    gainDatas.DeleteArrayElementAtIndex(list.index);
                    gainDatas.serializedObject.ApplyModifiedProperties();
                },
            };

            _lists[property.propertyPath] = list;
            return list;
        }

        private void InitMenu(SerializedProperty gainDatas)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("변수 아이템 획득"), false, () => CreateDataCallback(typeof(VariableGainShopItemData), gainDatas));
            menu.AddItem(new GUIContent("유닛 획득"), false, () => CreateDataCallback(typeof(UnitGainShopItemData), gainDatas));
            menu.AddItem(new GUIContent("아티팩트 획득"), false, () => CreateDataCallback(typeof(ArtifactGainShopItemData), gainDatas));
            menu.AddItem(new GUIContent("고서 획득"), false, () => CreateDataCallback(typeof(TomeGainShopItemData), gainDatas));

            menu.ShowAsContext();
        }

        private void CreateDataCallback(Type type, SerializedProperty gainDatas)
        {
            var instance = Activator.CreateInstance(type) as GainShopItemData;

            if (instance != null)
            {
                gainDatas.arraySize++;
                var newElement = gainDatas.GetArrayElementAtIndex(gainDatas.arraySize - 1);
                newElement.managedReferenceValue = instance;

                gainDatas.serializedObject.ApplyModifiedProperties();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var gainList = GetOrCreateDataList(property);

            float totalHeight = 170; 

            if (!property.FindPropertyRelative("_isCash").boolValue)
            {
                totalHeight += 20;
            }

            totalHeight += gainList.GetHeight();

            return totalHeight;
        }
    }
}