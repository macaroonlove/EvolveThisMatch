using EvolveThisMatch.Save;
using FrameWork.PlayFabExtensions;
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

        private UnityAction _onSelect;

        protected override void Initialize()
        {
            base.Initialize();

            BindObject(typeof(Objects));

            _gainItemGroup = GetObject((int)Objects.GainItemGroup).transform;
        }

        internal void Show(ShopSaveData.ShopCatalog shopCatalog, ShopItem itemData, List<UIShopRewardItem> shopRewardItems, UnityAction onSelect)
        {
            _onSelect = onSelect;

            base.Show(shopCatalog, itemData);

            #region 획득할 아이템 표시
            var rewards = itemData.rewards;
            for (int i = 0; i < shopRewardItems.Count; i++)
            {
                shopRewardItems[i].Show(rewards[i]);
                shopRewardItems[i].transform.parent = _gainItemGroup;
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

            _onSelect?.Invoke();
        }
    }
}