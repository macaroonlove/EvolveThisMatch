using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDefaultShopItem : UIBase
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
        }
        #endregion

        private Image _itemIcon;
        private TextMeshProUGUI _itemName;
        private TextMeshProUGUI _payText;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _itemIcon = GetImage((int)Images.ItemIcon);
            _itemName = GetText((int)Texts.ItemName);
            _payText = GetText((int)Texts.PayText);
        }

        internal void Show(ShopItemData itemData)
        {
            _itemIcon.sprite = itemData.itemIcon;
            _itemName.text = itemData.itemName;

            if (itemData.needCount == 0)
            {
                _payText.text = "무료";
            }
            else
            {
                if (itemData.isCash)
                {
                    _payText.text = $"￦ {itemData.needCount}";
                }
                else
                {
                    _payText.text = $"{itemData.variable} {itemData.needCount}";
                }
            }

            gameObject.SetActive(true);
            transform.parent.gameObject.SetActive(true);
        }

        public override void Hide(bool isForce = false)
        {
            gameObject.SetActive(false);
            transform.parent.gameObject.SetActive(false);
        }
    }
}