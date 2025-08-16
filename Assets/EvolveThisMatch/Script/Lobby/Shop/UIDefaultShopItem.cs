using FrameWork.UIBinding;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDefaultShopItem : UIBase, IPointerClickHandler
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

        private ShopItemData _data;
        private UnityAction<ShopItemData> _onSelect;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _itemIcon = GetImage((int)Images.ItemIcon);
            _itemName = GetText((int)Texts.ItemName);
            _payText = GetText((int)Texts.PayText);
        }

        internal void Show(ShopItemData itemData, UnityAction<ShopItemData> onSelect)
        {
            _data = itemData;
            _onSelect = onSelect;

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
                    _payText.text = $"<sprite name={itemData.variable.IconText}> {itemData.needCount}";
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

        public void OnPointerClick(PointerEventData eventData)
        {
            _onSelect?.Invoke(_data);
        }
    }
}