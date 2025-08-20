using System;
using System.Collections.Generic;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class DefaultShopData : ShopData
    {
        public override List<ShopItemData> GetItems()
        {
            return _shopItems;
        }
    }
}