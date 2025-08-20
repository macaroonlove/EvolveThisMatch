using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    [CreateAssetMenu(menuName = "Templates/Lobby/ShopItemData", fileName = "ShopItem_", order = 0)]
    public class ShopItemData : ScriptableObject
    {
        [SerializeField] private string _itemName;
        [SerializeField] private Sprite _itemIcon;

        [SerializeField] private bool _isCash;
        [SerializeField] private ObscuredIntVariable _variable;
        [SerializeField] private int _price;

        [SerializeField] private int _buyAbleCount;
        [SerializeField] private bool _isPackage;
        [SerializeField] private bool _isPanel;

        [Space(10)]
        [HideInInspector, SerializeReference] private List<GainShopItemData> _gainShopItemDatas = new List<GainShopItemData>();

        #region 프로퍼티
        public string itemName => _itemName;
        public Sprite itemIcon => _itemIcon;
        public bool isCash => _isCash;
        public ObscuredIntVariable variable => _variable;
        public int price => _price;
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

    [CustomEditor(typeof(ShopItemData), true)]
    public class ShopItemDataEditor : Editor
    {
        private ShopItemData _target;

        private SerializedProperty _itemName;
        private SerializedProperty _itemIcon;
        private SerializedProperty _isCash;
        private SerializedProperty _variable;
        private SerializedProperty _price;
        private SerializedProperty _buyAbleCount;
        private SerializedProperty _isPackage;
        private SerializedProperty _isPanel;
        private SerializedProperty _gainDatas;

        private ReorderableList _gainList;

        private void OnEnable()
        {
            _target = target as ShopItemData;

            _itemName = serializedObject.FindProperty("_itemName");
            _itemIcon = serializedObject.FindProperty("_itemIcon");
            _isCash = serializedObject.FindProperty("_isCash");
            _variable = serializedObject.FindProperty("_variable");
            _price = serializedObject.FindProperty("_price");
            _buyAbleCount = serializedObject.FindProperty("_buyAbleCount");
            _isPackage = serializedObject.FindProperty("_isPackage");
            _isPanel = serializedObject.FindProperty("_isPanel");
            _gainDatas = serializedObject.FindProperty("_gainShopItemDatas");

            CreateGainDataList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_itemName, new GUIContent("아이템 이름"));
            EditorGUILayout.PropertyField(_itemIcon, new GUIContent("아이템 이미지"));

            EditorGUILayout.Space(10);

            GUILayout.Label("결제", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_isCash, new GUIContent("현금 결제 여부"));

            if (!_isCash.boolValue)
            {
                EditorGUILayout.PropertyField(_variable, new GUIContent("필요 재화 종류"));
            }

            EditorGUILayout.PropertyField(_price, new GUIContent("가격"));

            EditorGUILayout.Space(10);

            GUILayout.Label("조건", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_buyAbleCount, new GUIContent("구매 가능 횟수"));
            EditorGUILayout.PropertyField(_isPackage, new GUIContent("패키지인가?"));
            EditorGUILayout.PropertyField(_isPanel, new GUIContent("패널이 열리는가?"));

            EditorGUILayout.Space(20);

            _gainList?.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        #region 리스트
        private void CreateGainDataList()
        {
            _gainList = new ReorderableList(serializedObject, _gainDatas, true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "획득 아이템");
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = _target.gainShopItemDatas[index];

                    if (element != null)
                    {
                        rect.y += 2;
                        rect.width -= 10;
                        rect.height = EditorGUIUtility.singleLineHeight;

                        // 타입 이름 출력
                        var labelRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                        EditorGUI.LabelField(labelRect, element.GetType().Name, EditorStyles.boldLabel);

                        rect.y += 20;

                        // 필드 출력
                        element.Draw(rect);
                    }
                },
                elementHeightCallback = (index) =>
                {
                    var element = _target.gainShopItemDatas[index];
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

                    _gainDatas.DeleteArrayElementAtIndex(list.index);
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                },
            };
        }

        private void InitMenu()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("변수 아이템 획득"), false, () => CreateDataCallback(typeof(VariableGainShopItemData)));
            menu.AddItem(new GUIContent("유닛 획득"), false, () => CreateDataCallback(typeof(UnitGainShopItemData)));
            menu.AddItem(new GUIContent("아티팩트 획득"), false, () => CreateDataCallback(typeof(ArtifactGainShopItemData)));
            menu.AddItem(new GUIContent("고서 획득"), false, () => CreateDataCallback(typeof(TomeGainShopItemData)));

            menu.ShowAsContext();
        }

        private void CreateDataCallback(Type type)
        {
            var instance = Activator.CreateInstance(type) as GainShopItemData;

            if (instance != null)
            {
                _gainDatas.arraySize++;
                var newElement = _gainDatas.GetArrayElementAtIndex(_gainDatas.arraySize - 1);
                newElement.managedReferenceValue = instance;

                _gainDatas.serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion
    }
}