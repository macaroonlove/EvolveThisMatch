using FrameWork;
using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIEngraveCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Toggles
        {
            EngraveToggle,
        }
        enum Buttons
        {
            LuckEngraveButton,
            DivineEngraveButton,
            DarkEngraveButton,
            FireEngraveButton,
            WaterEngraveButton,
            EarthEngraveButton,
            WindEngraveButton,
            ThunderEngraveButton,
        }
        enum CanvasGroup
        {
            EngravePanel,
        }
        #endregion

        private CanvasGroupController _panel;

        protected override void Initialize()
        {
            BindToggle(typeof(Toggles));
            BindButton(typeof(Buttons));
            BindCanvasGroupController(typeof(CanvasGroup));

            _panel = GetCanvasGroupController((int)CanvasGroup.EngravePanel);

            GetToggle((int)Toggles.EngraveToggle).onValueChanged.AddListener(ActivePanel);
            GetButton((int)Buttons.LuckEngraveButton).onClick.AddListener(LuckEngrave);
        }

        private void ActivePanel(bool isOn)
        {
            if (isOn)
            {
                _panel.Show();
            }
            else
            {
                _panel.Hide();
            }
        }

        private void LuckEngrave()
        {

        }
    }
}