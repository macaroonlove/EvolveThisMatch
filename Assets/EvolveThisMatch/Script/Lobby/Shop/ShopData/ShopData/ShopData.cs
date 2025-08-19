using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public abstract class ShopData
    {
        [SerializeField] private string _subTabName;

        [Header("보여줄 재화")]
        [SerializeField] private List<ObscuredIntVariable> _variableDisplays = new List<ObscuredIntVariable>();

        [Header("아이템")]
        [SerializeField] private List<ShopItemData> _shopItems = new List<ShopItemData>();

        #region 프로퍼티
        internal string subTabName => _subTabName;
        internal IReadOnlyList<ObscuredIntVariable> variableDisplays => _variableDisplays;
        internal IReadOnlyList<ShopItemData> shopItems => _shopItems;
        #endregion

        public abstract List<ShopItemData> GetItems();
    }
}