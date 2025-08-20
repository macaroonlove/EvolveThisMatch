using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIPackagePayPanel : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
            PayButton,
        }
        enum Images
        {
            PackageBG,
        }
        enum Texts
        {
            PayText,
            ItemName,
        }
        enum Objects
        {
            GainItemGroup,
        }
        #endregion

        [SerializeField] private GameObject _gainItemPrefab;

        private Image _packageBG;
        private TextMeshProUGUI _itemName;
        private TextMeshProUGUI _payText;
        private Transform _gainItemGroup;

        private List<UIShopGainItem> _shopGainItems = new List<UIShopGainItem>();

        private UnityAction _onPay;

        protected override void Initialize()
        {
            BindButton(typeof(Buttons));
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindObject(typeof(Objects));

            _packageBG = GetImage((int)Images.PackageBG);
            _itemName = GetText((int)Texts.ItemName);
            _payText = GetText((int)Texts.PayText);
            _gainItemGroup = GetObject((int)Objects.GainItemGroup).transform;

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
            GetButton((int)Buttons.PayButton).onClick.AddListener(Pay);
        }

        internal void Show(ShopItemData itemData, UnityAction onPay)
        {
            _onPay = onPay;

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

            base.Show(true);
        }

        private void Pay()
        {
            _onPay?.Invoke();

            Hide();
        }

        private void Hide()
        {
            base.Hide(true);
        }
    }
}