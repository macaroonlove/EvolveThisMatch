using FrameWork.UIBinding;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITomeEquipItem : UIBase, IPointerClickHandler
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Background,
            Icon,
            SelectDim,
        }
        #endregion

        private Image _background;
        private Image _icon;
        private Image _selectDim;

        private UnityAction _onClick;

        internal void Initialize(UnityAction onClick)
        {
            _onClick = onClick;

            BindImage(typeof(Images));

            _background = GetImage((int)Images.Background);
            _icon = GetImage((int)Images.Icon);
            _selectDim = GetImage((int)Images.SelectDim);
        }

        public void Render(TomeEquipItemViewState state)
        {
            if (!state.isEquip)
            {
                Hide();
                return;
            }

            _background.color = Color.white;
            _icon.sprite = state.icon;
            _icon.enabled = true;
        }

        private void Hide()
        {
            _background.color = Color.gray3;
            _icon.enabled = false;
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

    public readonly struct TomeEquipItemViewState
    {
        public readonly Sprite icon;
        public bool isEquip => icon != null;

        public TomeEquipItemViewState(Sprite icon)
        {
            this.icon = icon;
        }
    }
}