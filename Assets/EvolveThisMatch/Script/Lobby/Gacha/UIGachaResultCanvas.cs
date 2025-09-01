using FrameWork.UIBinding;

namespace EvolveThisMatch.Lobby
{
    public class UIGachaResultCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            ConfirmButton,
        }
        #endregion

        private UIGachaResultItem[] _uiGachaResultItems;

        protected override void Initialize()
        {
            BindButton(typeof(Buttons));

            _uiGachaResultItems = GetComponentsInChildren<UIGachaResultItem>();

            GetButton((int)Buttons.ConfirmButton).onClick.AddListener(Hide);
        }

        internal void Show(string[] results)
        {
            int count = results.Length;

            // 초기화
            for (int i = 0; i < _uiGachaResultItems.Length; i++)
            {
                if (i < count)
                {
                    _uiGachaResultItems[i].Show(results[i]);
                }
                else
                {
                    _uiGachaResultItems[i].Hide();
                }
            }

            base.Show(true);
        }

        internal void Hide()
        {
            base.Hide(true);
        }
    }
}