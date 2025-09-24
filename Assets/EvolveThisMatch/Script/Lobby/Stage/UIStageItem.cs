using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIStageItem : UIBase, IPointerClickHandler
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Background,
            Selected,
        }
        enum Texts
        {
            Title,
        }
        #endregion

        private Image _background;
        private Image _selected;
        private TextMeshProUGUI _title;

        private UnityAction<UIStageItem> _onSelect;

        internal WaveTemplate waveTemplate { get; private set; }

        internal void Initialize(UnityAction<UIStageItem> onSelect)
        {
            _onSelect = onSelect;

            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _background = GetImage((int)Images.Background);
            _selected = GetImage((int)Images.Selected);
            _title = GetText((int)Texts.Title);

            _selected.enabled = false;
        }

        internal void Show(WaveTemplate waveTemplate, bool isActive)
        {
            this.waveTemplate = waveTemplate;

            _title.text = $"{waveTemplate.stage}\n{waveTemplate.displayName}";
            _background.color = isActive ? Color.yellow : Color.white;
        }

        internal void Select()
        {
            _selected.enabled = true;
        }

        internal void DeSelect()
        {
            _selected.enabled = false;
        }

        internal void Click()
        {
            _onSelect?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Click();
        }
    }
}