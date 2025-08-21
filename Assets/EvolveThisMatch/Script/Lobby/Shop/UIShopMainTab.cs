using DG.Tweening;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIShopMainTab : UIBase
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
        private TextMeshProUGUI _mainTabName;

        private ShopDataTemplate _template;
        private UnityAction<UIShopMainTab> _onSelect;

        internal ShopDataTemplate template => _template;

        internal void Initialize(ShopDataTemplate template, UnityAction<UIShopMainTab> onSelect)
        {
            _template = template;
            _onSelect = onSelect;

            BindImage(typeof(Images));
            BindText(typeof(Texts));

            if (TryGetComponent(out Button button))
            {
                button.onClick.AddListener(OnClick);
            }

            _background = GetImage((int)Images.Background);
            _mainTabName = GetText((int)Texts.Title);

            _mainTabName.text = template.mainTabName;
        }

        private void OnClick()
        {
            _onSelect?.Invoke(this);
        }

        internal virtual void Select()
        {
            transform.DOLocalMoveX(60, 0.5f);

            _background.color = _selectColor;
            _mainTabName.color = _unSelectColor;
        }

        internal virtual void UnSelect()
        {
            transform.DOLocalMoveX(20, 0.5f);

            _background.color = _unSelectColor;
            _mainTabName.color = _selectColor;
        }
    }
}