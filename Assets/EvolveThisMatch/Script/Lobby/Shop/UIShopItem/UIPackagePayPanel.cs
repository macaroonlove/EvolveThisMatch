using EvolveThisMatch.Save;
using FrameWork.PlayFabExtensions;
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
            RewardItemGroup,
        }
        #endregion

        [SerializeField] private GameObject _rewardItemPrefab;

        private Transform _rewardItemGroup;

        private List<UIShopRewardItem> _shopRewardItems = new List<UIShopRewardItem>();

        private UnityAction _onPay;

        protected override void Initialize()
        {
            base.Initialize();

            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _rewardItemGroup = GetObject((int)Objects.RewardItemGroup).transform;

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
            GetButton((int)Buttons.PayButton).onClick.AddListener(Pay);
        }

        internal void Show(ShopSaveData.ShopCatalog shopCatalog, ShopItem itemData, UnityAction onPay)
        {
            _onPay = onPay;

            base.Show(shopCatalog, itemData);

            #region 획득할 아이템 표시
            foreach (var rewardItem in _shopRewardItems) rewardItem.Hide(true);

            var rewards = itemData.rewards;
            for (int i = 0; i < rewards.Count; i++)
            {
                UIShopRewardItem rewardItem;
                if (i < _shopRewardItems.Count)
                {
                    rewardItem = _shopRewardItems[i];
                }
                else
                {
                    var instance = Instantiate(_rewardItemPrefab, _rewardItemGroup);
                    rewardItem = instance.GetComponent<UIShopRewardItem>();

                    _shopRewardItems.Add(rewardItem);
                }

                rewardItem.Show(rewards[i]);
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