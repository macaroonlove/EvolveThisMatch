using EvolveThisMatch.Save;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace EvolveThisMatch.Lobby
{
    public class UIPackageShopItem : UIShopItem, IPointerClickHandler
    {
        #region 바인딩
        enum Objects
        {
            GainItemGroup,
        }
        #endregion

        private Transform _gainItemGroup;

        private UnityAction<ShopSaveData.ShopItem> _onSelect;

        protected override void Initialize()
        {
            base.Initialize();

            BindObject(typeof(Objects));

            _gainItemGroup = GetObject((int)Objects.GainItemGroup).transform;
        }

        internal void Show(ShopSaveData.ShopCatalog shopCatalog, ShopItemData itemData, List<UIShopGainItem> gainItems, UnityAction<ShopSaveData.ShopItem> onSelect)
        {
            _onSelect = onSelect;

            base.Show(shopCatalog, itemData);

            #region 획득할 아이템 표시
            var gainDatas = itemData.gainShopItemDatas;
            for (int i = 0; i < gainItems.Count; i++)
            {
                gainItems[i].Show(gainDatas[i]);
                gainItems[i].transform.parent = _gainItemGroup;
            }
            #endregion

            gameObject.SetActive(true);
        }

        public override void Hide(bool isForce = false)
        {
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isBuyAble) return;

            _onSelect?.Invoke(_shopItem);
        }
    }
}