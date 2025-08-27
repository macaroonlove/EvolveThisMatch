using FrameWork.PlayFabExtensions;
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

        private ShopSubTab _subTabData;
        private UnityAction<UIShopSubTab> _onSelect;

        internal ShopSubTab subTabData => _subTabData;

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

        internal void Show(ShopSubTab subTabData, UnityAction<UIShopSubTab> onSelect)
        {
            _subTabData = subTabData;
            _onSelect = onSelect;

            _subTabName.text = subTabData.subTab;

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