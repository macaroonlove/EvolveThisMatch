using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDefaultPayPanel : UIBase
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
        }
        enum Sliders
        {
            CounterSlider,
        }
        #endregion

        private Image _itemIcon;
        private TextMeshProUGUI _itemName;
        private TextMeshProUGUI _payText;
        private TextMeshProUGUI _counterText;
        private Slider _counterSlider;

        private UnityAction<int> _onPay;

        protected override void Initialize()
        {
            BindButton(typeof(Buttons));
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindSlider(typeof(Sliders));

            _itemIcon = GetImage((int)Images.ItemIcon);
            _itemName = GetText((int)Texts.ItemName);
            _payText = GetText((int)Texts.PayText);
            _counterText = GetText((int)Texts.CounterText);
            _counterSlider = GetSlider((int)Sliders.CounterSlider);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
            GetButton((int)Buttons.PayButton).onClick.AddListener(Pay);
            GetButton((int)Buttons.MinusButton).onClick.AddListener(() => _counterSlider.value--);
            GetButton((int)Buttons.PlusButton).onClick.AddListener(() => _counterSlider.value++);

            _counterSlider.onValueChanged.AddListener(ChangeCounter);
        }

        internal void Show(ShopItemData itemData, UnityAction<int> onPay)
        {
            _onPay = onPay;

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

            if (itemData.isCash)
            {
                _counterText.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                _counterText.transform.parent.gameObject.SetActive(true);
                
                int maxValue = itemData.variable.Value / itemData.needCount;
                _counterSlider.maxValue = maxValue;

                _counterSlider.value = 1;
                ChangeCounter(1);
            }

            base.Show(true);
        }

        private void ChangeCounter(float value)
        {
            _counterText.text = $"{value} / {_counterSlider.maxValue} 개";
        }

        private void Pay()
        {
            _onPay?.Invoke((int)_counterSlider.value);

            Hide();
        }

        private void Hide()
        {
            base.Hide(true);
        }
    }
}
