using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITomeInfoView : UIBase
    {
        #region 바인딩
        enum Texts
        {
            DisplayName,
            Description,
            NeedCoinValue,
            CooldownTimeValue,
            RangeValue,
        }
        enum Images
        {
            Icon,
            VideoImage,
        }
        enum CanvasGroups
        {
            Info,
        }
        #endregion

        private TextMeshProUGUI _displayName;
        private TextMeshProUGUI _description;
        private TextMeshProUGUI _needCoinValue;
        private TextMeshProUGUI _cooldownTimeValue;
        private TextMeshProUGUI _rangeValue;
        private Image _icon;
        private Image _videoImage;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindCanvasGroupController(typeof(CanvasGroups));

            _displayName = GetText((int)Texts.DisplayName);
            _description = GetText((int)Texts.Description);
            _needCoinValue = GetText((int)Texts.NeedCoinValue);
            _cooldownTimeValue = GetText((int)Texts.CooldownTimeValue);
            _rangeValue = GetText((int)Texts.RangeValue);
            _icon = GetImage((int)Images.Icon);
            _videoImage = GetImage((int)Images.VideoImage);
        }

        internal void Show(TomeInfoViewState state)
        {
            _displayName.text = state.displayName;
            _description.text = state.description;
            _icon.sprite = state.icon;
            _needCoinValue.text = $"{state.needCoin} 개";
            _cooldownTimeValue.text = $"{state.cooldownTime} 초";
            _rangeValue.text = state.range;
        }
    }

    public readonly struct TomeInfoViewState
    {
        public readonly string displayName;
        public readonly string description;
        public readonly Sprite icon;
        public readonly int needCoin;
        public readonly float cooldownTime;
        public readonly string range;

        public TomeInfoViewState(string displayName, string description, Sprite icon, int needCoin, float cooldownTime, string range)
        {
            this.displayName = displayName;
            this.description = description;
            this.icon = icon;
            this.needCoin = needCoin;
            this.cooldownTime = cooldownTime;
            this.range = range;
        }
    }
}