using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
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
        }
        #endregion

        protected Image _itemIcon;
        protected TextMeshProUGUI _itemName;
        protected TextMeshProUGUI _payText;
        protected TextMeshProUGUI _purchaseLimitText;
        protected CanvasGroupController _purchaseLimit;
        
        protected ShopSaveData.ShopItem _shopItem;
        protected bool _isBuyAble;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindCanvasGroupController(typeof(CanvasGroups));

            _itemIcon = GetImage((int)Images.ItemIcon);
            _itemName = GetText((int)Texts.ItemName);
            _payText = GetText((int)Texts.PayText);
            _purchaseLimitText = GetText((int)Texts.PurchaseLimitText);
            _purchaseLimit = GetCanvasGroupController((int)CanvasGroups.PurchaseLimit);
        }

        protected void Show(ShopSaveData.ShopCatalog shopCatalog, ShopItemData itemData)
        {
            _itemIcon.sprite = itemData.itemIcon;
            _itemName.text = itemData.itemName;

            int price = itemData.price;
            if (price == 0)
            {
                _payText.text = "무료";
            }
            else
            {
                if (itemData.isCash)
                {
                    _payText.text = $"￦ {price}";
                }
                else
                {
                    _payText.text = $"<sprite name={itemData.variable.IconText}> {price}";
                }
            }

            _shopItem = null;
            _isBuyAble = true;

            // 구매 횟수 제한이 있다면
            if (itemData.buyAbleCount > 0)
            {
                _purchaseLimit.Show(true);
                _shopItem = shopCatalog.GetItem(itemData.itemName);

                if (_shopItem != null)
                {
                    int remainCount = itemData.buyAbleCount - _shopItem.boughtCount;
                    _purchaseLimitText.text = $"{remainCount}/{itemData.buyAbleCount}";

                    _isBuyAble = remainCount > 0;
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
    }
}
