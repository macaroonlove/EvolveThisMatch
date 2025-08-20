using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIPackageShopItem : UIBase, IPointerClickHandler
    {
        #region 바인딩
        enum Images
        {
            PackageBG,
        }
        enum Texts
        {
            ItemName,
            PayText,
        }
        enum Objects
        {
            GainItemGroup,
        }
        #endregion

        private Image _packageBG;
        private TextMeshProUGUI _itemName;
        private TextMeshProUGUI _payText;
        private Transform _gainItemGroup;

        private ShopItemData _data;
        private UnityAction<ShopItemData> _onSelect;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindObject(typeof(Objects));

            _packageBG = GetImage((int)Images.PackageBG);
            _itemName = GetText((int)Texts.ItemName);
            _payText = GetText((int)Texts.PayText);
            _gainItemGroup = GetObject((int)Objects.GainItemGroup).transform;
        }

        internal void Show(ShopItemData itemData, List<UIShopGainItem> gainItems, UnityAction<ShopItemData> onSelect)
        {
            _data = itemData;
            _onSelect = onSelect;

            _packageBG.sprite = itemData.itemIcon;
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

            var gainDatas = itemData.gainShopItemDatas;
            for (int i = 0; i < gainItems.Count; i++)
            {
                gainItems[i].Show(gainDatas[i]);
                gainItems[i].transform.parent = _gainItemGroup;
            }

            gameObject.SetActive(true);
        }

        public override void Hide(bool isForce = false)
        {
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onSelect?.Invoke(_data);
        }
    }
}