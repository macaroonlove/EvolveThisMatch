using DG.Tweening;
using FrameWork.UIBinding;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIGachaTab : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            GachaTab,
        }

        enum Texts
        {
            GachaTabText,
        }
        #endregion

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

            GetImage((int)Images.GachaTab).sprite = gachaData.tabBackground;
            GetText((int)Texts.GachaTabText).text = gachaData.tabName;
        }

        private void OnClick()
        {
            _onSelect?.Invoke(this);
        }

        internal virtual void Select()
        {
            transform.DOLocalMoveX(0, 0.5f);
        }

        internal virtual void UnSelect()
        {
            transform.DOLocalMoveX(-20, 0.5f);
        }
    }
}