using EvolveThisMatch.Save;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIPackagePayPanel : UIShopItem
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
            PayButton,
        }
        enum Objects
        {
            GainItemGroup,
        }
        #endregion

        [SerializeField] private GameObject _gainItemPrefab;

        private Transform _gainItemGroup;

        private List<UIShopGainItem> _shopGainItems = new List<UIShopGainItem>();

        private UnityAction _onPay;

        protected override void Initialize()
        {
            base.Initialize();

            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _gainItemGroup = GetObject((int)Objects.GainItemGroup).transform;

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
            GetButton((int)Buttons.PayButton).onClick.AddListener(Pay);
        }

        internal void Show(ShopSaveData.ShopCatalog shopCatalog, ShopItemData itemData, UnityAction onPay)
        {
            _onPay = onPay;

            base.Show(shopCatalog, itemData);

            #region 획득할 아이템 표시
            foreach (var gainItem in _shopGainItems) gainItem.Hide(true);

            var gainDatas = itemData.gainShopItemDatas;
            for (int i = 0; i < gainDatas.Count; i++)
            {
                UIShopGainItem gainItem;
                if (i < _shopGainItems.Count)
                {
                    gainItem = _shopGainItems[i];
                }
                else
                {
                    var instance = Instantiate(_gainItemPrefab, _gainItemGroup);
                    gainItem = instance.GetComponent<UIShopGainItem>();

                    _shopGainItems.Add(gainItem);
                }

                gainItem.Show(gainDatas[i]);
            }
            #endregion

            base.Show(true);
        }

        private void Pay()
        {
            if (!_isBuyAble) return;

            _onPay?.Invoke();

            Hide();
        }

        private void Hide()
        {
            base.Hide(true);
        }
    }
}