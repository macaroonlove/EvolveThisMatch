using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIGachaButton : UIBase
    {
        #region 바인딩
        enum Texts
        {
            Title,
            NeedText,
        }

        enum Images
        {
            GachaButton,
            Icon,
        }
        #endregion

        private TextMeshProUGUI _titleText;
        private TextMeshProUGUI _needText;
        private Image _background;
        private Image _icon;

        private int _gachaCount;
        private int _needCount;
        private ObscuredIntVariable _variable;
        private UnityAction<int, int, ObscuredIntVariable> _action;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _titleText = GetText((int)Texts.Title);
            _needText = GetText((int)Texts.NeedText);
            _background = GetImage((int)Images.GachaButton);
            _icon = GetImage((int)Images.Icon);

            if (TryGetComponent(out Button button))
            {
                button.onClick.AddListener(OnClick);
            }
        }

        internal void Show(int gachaCount, int needCount, Color color, ObscuredIntVariable variable, UnityAction<int, int, ObscuredIntVariable> action)
        {
            _variable = variable;
            _gachaCount = gachaCount;
            _needCount = needCount;
            _action = action;

            _background.color = color;
            _icon.sprite = variable.Icon;
            _titleText.text = $"{gachaCount}회 소환";
            _needText.text = needCount.ToString();
        }

        private void OnClick()
        {
            _action?.Invoke(_gachaCount, _needCount, _variable);
        }
    }
}