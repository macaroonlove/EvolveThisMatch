using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.PlayFabExtensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDefaultPayPanel : UIShopItem
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
            PayButton,
            MinusButton,
            PlusButton,
        }
        enum Images
        {
            ItemIcon,
        }
        enum Texts
        {
            ItemName,
            PayText,
            CounterText,
            PurchaseLimitText,
        }
        enum Sliders
        {
            CounterSlider,
        }
        enum CanvasGroups
        {
            PurchaseLimit,
        }
        #endregion

        private TextMeshProUGUI _counterText;
        private Slider _counterSlider;

        private CurrencySystem _currencySystem;

        private UnityAction<int> _onPay;

        protected override void Initialize()
        {
            _currencySystem = CoreManager.Instance.GetSubSystem<CurrencySystem>();

            BindButton(typeof(Buttons));
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindSlider(typeof(Sliders));
            BindCanvasGroupController(typeof(CanvasGroups));

            _itemIcon = GetImage((int)Images.ItemIcon);
            _itemName = GetText((int)Texts.ItemName);
            _payText = GetText((int)Texts.PayText);
            _counterText = GetText((int)Texts.CounterText);
            _purchaseLimitText = GetText((int)Texts.PurchaseLimitText);
            _counterSlider = GetSlider((int)Sliders.CounterSlider);
            _purchaseLimit = GetCanvasGroupController((int)CanvasGroups.PurchaseLimit);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
            GetButton((int)Buttons.PayButton).onClick.AddListener(Pay);
            GetButton((int)Buttons.MinusButton).onClick.AddListener(() => _counterSlider.value--);
            GetButton((int)Buttons.PlusButton).onClick.AddListener(() => _counterSlider.value++);

            _counterSlider.onValueChanged.AddListener(ChangeCounter);
        }

        internal void Show(ShopSaveData.ShopCatalog shopCatalog, ShopItem itemData, UnityAction<int> onPay)
        {
            _onPay = onPay;

            base.Show(shopCatalog, itemData);

            #region 아이템 구매 횟수 결정
            if (itemData.currency == "RM")
            {
                _counterText.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                _counterText.transform.parent.gameObject.SetActive(true);

                if (Enum.TryParse<CurrencyType>(itemData.currency, true, out var currency))
                {
                    var value = _currencySystem.GetAmount(currency);

                    int maxValue = value / itemData.price;
                    maxValue = Mathf.Min(maxValue, itemData.buyAbleCount);
                    _counterSlider.maxValue = maxValue;
                }
                
                _counterSlider.value = 1;
                ChangeCounter(1);
            }
            #endregion

            base.Show(true);
        }

        private void ChangeCounter(float value)
        {
            _counterText.text = $"{value} / {_counterSlider.maxValue} 개";
        }

        private void Pay()
        {
            if (!_isBuyAble) return;

            _onPay?.Invoke((int)_counterSlider.value);

            Hide();
        }

        private void Hide()
        {
            base.Hide(true);
        }
    }
}
