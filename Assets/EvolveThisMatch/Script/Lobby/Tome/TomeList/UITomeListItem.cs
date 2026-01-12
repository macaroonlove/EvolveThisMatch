using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITomeListItem : UIBase, IPointerClickHandler
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Level,
            DisplayName,
            CounterText,
        }
        enum Images
        {
            Icon,
            CounterImage,
            SelectDim,
        }
        #endregion

        private TextMeshProUGUI _level;
        private TextMeshProUGUI _displayName;
        private TextMeshProUGUI _counterText;
        private Image _icon;
        private Image _counterImage;
        private Image _selectDim;

        private UnityAction _onClick;

        internal void Initialize(UnityAction onClick)
        {
            _onClick = onClick;

            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _level = GetText((int)Texts.Level);
            _displayName = GetText((int)Texts.DisplayName);
            _counterText = GetText((int)Texts.CounterText);
            _icon = GetImage((int)Images.Icon);
            _counterImage = GetImage((int)Images.CounterImage);
            _selectDim = GetImage((int)Images.SelectDim);
        }

        public void Render(TomeListItemViewState state)
        {
            gameObject.SetActive(state.isVisible);
            if (!state.isVisible) return;

            _icon.sprite = state.icon;
            _displayName.text = state.displayName;
            _level.text = $"Lv. {state.level}";
            _counterText.text = state.countText;
            _counterImage.fillAmount = state.countProgress;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke();
        }

        internal virtual void Select(bool selected)
        {
            _selectDim.enabled = selected;
        }
    }

    public readonly struct TomeListItemViewState
    {
        public readonly bool isVisible;
        public readonly Sprite icon;
        public readonly string displayName;
        public readonly int level;
        public readonly string countText;
        public readonly float countProgress;

        public TomeListItemViewState(bool isVisible, Sprite icon, string displayName, int level, string countText, float countProgress)
        {
            this.isVisible = isVisible;
            this.icon = icon;
            this.displayName = displayName;
            this.level = level;
            this.countText = countText;
            this.countProgress = countProgress;
        }
    }
}