using DG.Tweening;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIGachaTab : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Background,
        }

        enum Texts
        {
            TabName,
        }
        #endregion

        [SerializeField] private Color _selectColor;
        [SerializeField] private Color _unSelectColor;

        private Image _background;
        private TextMeshProUGUI _tabName;

        private GachaData _gachaData;
        private UnityAction<UIGachaTab> _onSelect;

        internal GachaData gachaData => _gachaData;

        internal void Initialize(GachaData gachaData, UnityAction<UIGachaTab> onSelect)
        {
            _gachaData = gachaData;
            _onSelect = onSelect;

            BindImage(typeof(Images));
            BindText(typeof(Texts));

            if (TryGetComponent(out Button button))
            {
                button.onClick.AddListener(OnClick);
            }

            _background = GetImage((int)Images.Background); 
            _tabName = GetText((int)Texts.TabName); 

            _background.sprite = gachaData.tabBackground;
            _tabName.text = gachaData.tabName;
        }

        private void OnClick()
        {
            _onSelect?.Invoke(this);
        }

        internal virtual void Select()
        {
            transform.DOLocalMoveX(60, 0.5f);

            _background.color = _selectColor;
            _tabName.color = _unSelectColor;
        }

        internal virtual void UnSelect()
        {
            transform.DOLocalMoveX(20, 0.5f);

            _background.color = _unSelectColor;
            _tabName.color = _selectColor;
        }
    }
}