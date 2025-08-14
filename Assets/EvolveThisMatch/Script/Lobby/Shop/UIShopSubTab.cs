using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIShopSubTab : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Background,
        }

        enum Texts
        {
            Title,
        }
        #endregion

        [SerializeField] private Color _selectColor;
        [SerializeField] private Color _unSelectColor;

        private Image _background;
        private TextMeshProUGUI _subTabName;

        private ShopData _data;
        private UnityAction<UIShopSubTab> _onSelect;

        internal ShopData data => _data;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            if (TryGetComponent(out Button button))
            {
                button.onClick.AddListener(OnClick);
            }

            _subTabName = GetText((int)Texts.Title);
            _background = GetImage((int)Images.Background);
        }

        internal void Show(ShopData data, UnityAction<UIShopSubTab> onSelect)
        {
            _data = data;
            _onSelect = onSelect;

            _subTabName.text = data.subTabName;

            base.Show(true);
        }

        private void OnClick()
        {
            Select();
        }

        internal virtual void Select()
        {
            _onSelect?.Invoke(this);

            _subTabName.color = _selectColor;
            _background.enabled = true;
        }

        internal virtual void UnSelect()
        {
            _subTabName.color = _unSelectColor;
            _background.enabled = false;
        }
    }
}