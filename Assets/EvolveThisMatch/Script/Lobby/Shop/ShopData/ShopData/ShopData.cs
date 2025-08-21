using FrameWork.NetworkTime;
using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public abstract class ShopData : ScriptableObject
    {
        [SerializeField] private string _subTabName;

        [SerializeField] private ECycleType _cycleType;
        [SerializeField] private int _cycleInterval;

        [SerializeField] private List<ObscuredIntVariable> _variableDisplays = new List<ObscuredIntVariable>();
        [SerializeField] protected List<ShopItemData> _shopItems = new List<ShopItemData>();

        #region 프로퍼티
        internal string subTabName => _subTabName;
        internal ECycleType cycleType => _cycleType;
        internal int cycleInterval => _cycleInterval;
        internal IReadOnlyList<ObscuredIntVariable> variableDisplays => _variableDisplays;
        #endregion

        public abstract List<ShopItemData> GetItems();

#if UNITY_EDITOR
        private ReorderableList _variableList;
        private ReorderableList _itemList;

        public void InitLists()
        {
            if (_variableDisplays == null) _variableDisplays = new List<ObscuredIntVariable>();
            if (_shopItems == null) _shopItems = new List<ShopItemData>();

            _variableList = new ReorderableList(_variableDisplays, typeof(ObscuredIntVariable), true, true, true, true);
            _variableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "보여질 Variable Displays");
            _variableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                _variableDisplays[index] = (ObscuredIntVariable)EditorGUI.ObjectField(rect, _variableDisplays[index], typeof(ObscuredIntVariable), false);
            };

            _itemList = new ReorderableList(_shopItems, typeof(ShopItemData), true, true, true, true);
            _itemList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "상점 아이템들");
            _itemList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                _shopItems[index] = (ShopItemData)EditorGUI.ObjectField(rect, _shopItems[index], typeof(ShopItemData), false);
            };
        }

        public virtual void Draw(Rect rect)
        {
            if (_variableList == null || _itemList == null) InitLists();

            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "서브 탭 이름");
            _subTabName = EditorGUI.TextField(valueRect, _subTabName);

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "아이템 초기화 단위");
            _cycleType = (ECycleType)EditorGUI.EnumPopup(valueRect, _cycleType);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "아이템 초기화 주기");
            _cycleInterval = EditorGUI.IntField(valueRect, _cycleInterval);

            rect.y = labelRect.y + 25;

            // Variable Displays
            _variableList.DoList(new Rect(rect.x, rect.y, rect.width, _variableList.GetHeight()));

            rect.y += _variableList.GetHeight() + 10;

            // Shop Items
            _itemList.DoList(new Rect(rect.x, rect.y, rect.width, _itemList.GetHeight()));
        }

        public virtual float GetHeight()
        {
            if (_variableList == null || _itemList == null) InitLists();

            float height = 120;

            height += _variableList.GetHeight() + 10;
            height += _itemList.GetHeight();

            return height;
        }
#endif
    }
}