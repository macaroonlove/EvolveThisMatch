using FrameWork.UIBinding;
using System.Collections.Generic;
using EvolveThisMatch.Core;

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

        internal void Show(List<AgentTemplate> agents)
        {
            int count = agents.Count;

            // 초기화
            for (int i = 0; i < _uiGachaResultItems.Length; i++)
            {
                if (i < count)
                {
                    _uiGachaResultItems[i].Show(agents[i]);
                }
                else
                {
                    _uiGachaResultItems[i].Hide();
                }
            }

            base.Show(true);
        }

        internal void Show(List<SkinTemplate> skins)
        {
            int count = skins.Count;

            // 초기화
            for (int i = 0; i < _uiGachaResultItems.Length; i++)
            {
                if (i < count)
                {
                    _uiGachaResultItems[i].Show(skins[i]);
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