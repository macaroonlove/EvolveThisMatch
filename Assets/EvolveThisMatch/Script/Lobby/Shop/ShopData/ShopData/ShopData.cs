using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public abstract class ShopData
    {
        [SerializeField] private string _subTabName;

        [Header("아이템")]
        [SerializeField] private List<ShopItemData> _shopItems = new List<ShopItemData>();

        #region 프로퍼티
        internal string subTabName => _subTabName;
        internal IReadOnlyList<ShopItemData> shopItems => _shopItems;
        #endregion

        public abstract List<ShopItemData> GetItems();
    }
}