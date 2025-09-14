using FrameWork.UIBinding;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDepartmentItem : UIBase, IPointerClickHandler
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            ItemBackground,
            DepartmentIcon,
        }
        enum Texts
        {
            Title,
        }
        #endregion

        [SerializeField] private Color _selectColor;
        [SerializeField] private Color _deSelectColor;

        private Image _itemBackground;
        private UnityAction _action;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _itemBackground = GetImage((int)Images.ItemBackground);
        }

        internal void Show(DepartmentTemplate template, UnityAction action)
        {
            GetImage((int)Images.DepartmentIcon).sprite = template.departmentBackground;
            GetText((int)Texts.Title).text = template.departmentName;

            _action = action;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SelectItem();
        }

        internal void SelectItem()
        {
            _action?.Invoke();

            _itemBackground.color = _selectColor;
        }

        internal void DeSelectItem()
        {
            _itemBackground.color = _deSelectColor;
        }
    }
}