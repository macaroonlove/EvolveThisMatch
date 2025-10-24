using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIGachaAdButton : UIBase
    {
        #region 바인딩
        enum Texts
        {
            Title,
            CostText,
        }

        enum Images
        {
            Background,
        }
        #endregion

        private TextMeshProUGUI _titleText;
        private TextMeshProUGUI _costText;
        private Button _button;
        private Image _background;

        private int _gachaCount;
        private int _itemIndex;
        private UnityAction<int, int> _action;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _titleText = GetText((int)Texts.Title);
            _costText = GetText((int)Texts.CostText);
            _background = GetImage((int)Images.Background);

            if (TryGetComponent(out _button))
            {
                _button.onClick.AddListener(OnClick);
            }
        }

        internal void Show(int gachaCount, int boughtCount, int buyAbleCount, string colorString, int itemIndex, UnityAction<int, int> action)
        {
            _gachaCount = gachaCount;
            _itemIndex = itemIndex;
            _action = action;

            _titleText.text = $"{gachaCount}회 소환";

            if (ColorUtility.TryParseHtmlString(colorString, out Color color))
            {
                _background.color = color;
            }

            // 광고를 본 횟수 < 광고를 볼 수 있는 횟수
            bool isGachaAble = boughtCount < buyAbleCount;
            _button.interactable = isGachaAble;

            _costText.text = $"<sprite name=Ad> {buyAbleCount - boughtCount} / {buyAbleCount}";
            gameObject.SetActive(true);
        }

        public override void Hide(bool isForce = false)
        {
            gameObject.SetActive(false);
        }

        private void OnClick()
        {
            _action?.Invoke(_gachaCount, _itemIndex);
        }
    }
}