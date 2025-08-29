using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIShopItem : UIBase
    {
        #region 바인딩
        enum Images
        {
            ItemIcon,
        }
        enum Texts
        {
            ItemName,
            PayText,
            PurchaseLimitText,
        }
        enum CanvasGroups
        {
            PurchaseLimit,
            BuyComplete,
        }
        #endregion

        protected Image _itemIcon;
        protected TextMeshProUGUI _itemName;
        protected TextMeshProUGUI _payText;
        protected TextMeshProUGUI _purchaseLimitText;
        protected CanvasGroupController _purchaseLimit;
        protected CanvasGroupController _buyComplete;

        protected CurrencySystem _currencySystem;
        protected ShopSaveData.ShopItem _shopItem;
        protected bool _isBuyAble;

        protected override void Initialize()
        {
            _currencySystem = CoreManager.Instance.GetSubSystem<CurrencySystem>();

            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindCanvasGroupController(typeof(CanvasGroups));

            _itemIcon = GetImage((int)Images.ItemIcon);
            _itemName = GetText((int)Texts.ItemName);
            _payText = GetText((int)Texts.PayText);
            _purchaseLimitText = GetText((int)Texts.PurchaseLimitText);
            _purchaseLimit = GetCanvasGroupController((int)CanvasGroups.PurchaseLimit);
            _buyComplete = GetCanvasGroupController((int)CanvasGroups.BuyComplete);
        }

        protected void Show(ShopSaveData.ShopCatalog shopCatalog, ShopItem itemData)
        {
            AddressableAssetManager.Instance.GetSprite(itemData.icon, (sprite) => {
                _itemIcon.sprite = sprite;
            });

            _itemName.text = itemData.displayName;

            int price = itemData.price;
            if (price == 0)
            {
                _payText.text = "무료";
            }
            else
            {
                if (itemData.currency == "RM")
                {
                    _payText.text = $"￦ {price}";
                }
                else
                {
                    string iconText = UpperFirst(itemData.currency);
                    var isPayAble = IsPayAble(itemData);
                    if (isPayAble) _payText.text = $"<sprite name={iconText}> {price}";
                    else _payText.text = $"<color=red><sprite name={iconText}> {price}</color>";
                }
            }

            _shopItem = null;
            _isBuyAble = true;

            _buyComplete?.Hide(true);

            // 구매 횟수 제한이 있다면
            if (itemData.buyAbleCount > 0)
            {
                _purchaseLimit.Show(true);
                _shopItem = shopCatalog.GetItem(itemData.id);

                if (_shopItem != null)
                {
                    int remainCount = itemData.buyAbleCount - _shopItem.BoughtCount;
                    _purchaseLimitText.text = $"{remainCount}/{itemData.buyAbleCount}";

                    if (remainCount <= 0)
                    {
                        _isBuyAble = false;
                        _buyComplete?.Show(true);
                    }
                }
                else
                {
                    _purchaseLimitText.text = $"{itemData.buyAbleCount}/{itemData.buyAbleCount}";
                }
            }
            else
            {
                _purchaseLimit.Hide(true);
            }
        }

        /// <summary>
        /// 구매 가능 여부
        /// </summary>
        private bool IsPayAble(ShopItem itemData)
        {

            if (itemData.currency != "RM" && itemData.price != 0)
            {
                if (Enum.TryParse<CurrencyType>(itemData.currency, true, out var currency))
                {
                    var value = _currencySystem.GetAmount(currency);
                    int maxValue = value / itemData.price;

                    if (maxValue <= 0)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public string UpperFirst(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
