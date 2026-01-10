using FrameWork;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDepartmentListItem : UIBase, IPointerClickHandler
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
        private Image _departmentIcon;
        private TextMeshProUGUI _title;

        private UnityAction _action;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _itemBackground = GetImage((int)Images.ItemBackground);
            _departmentIcon = GetImage((int)Images.DepartmentIcon);
            _title = GetText((int)Texts.Title);
        }

        internal void Show(DepartmentData titleData, UnityAction action)
        {
            AddressableAssetManager.Instance.GetSprite(titleData.Background, (sprite) =>
            {
                _departmentIcon.sprite = sprite;
            });

            _title.text = titleData.DepartmentName;
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