using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
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

        internal int index { get; private set; }
        internal int listItemIndex { get; private set; }
        internal TomeTemplate template { get; private set; }
        internal ProfileSaveData.Tome owned { get; private set; }

        private UnityAction<UITomeEquipItem> _action;

        internal virtual void Initialize(int index, UnityAction<UITomeEquipItem> action = null)
        {
            this.index = index;
            this.listItemIndex = -1;
            _action = action;

            BindImage(typeof(Images));

            _background = GetImage((int)Images.Background);
            _icon = GetImage((int)Images.Icon);
            _selectDim = GetImage((int)Images.SelectDim);
        }

        internal virtual void Show(TomeTemplate template, ProfileSaveData.Tome owned, int listItemIndex)
        {
            this.template = template;
            this.owned = owned;
            this.listItemIndex = listItemIndex;

            _background.color = Color.white;
            _icon.sprite = template.sprite;
            _icon.enabled = true;
        }

        internal void Hide()
        {
            this.template = null;
            this.owned = null;
            this.listItemIndex = -1;
            _background.color = Color.gray3;
            _icon.enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SelectItem();
        }

        internal virtual void SelectItem()
        {
            _action?.Invoke(this);

            _selectDim.enabled = true;
        }

        internal virtual void DeSelectItem()
        {
            _selectDim.enabled = false;
        }
    }
}
