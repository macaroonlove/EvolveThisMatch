using EvolveThisMatch.Core;
using FrameWork;
using FrameWork.UIBinding;

namespace EvolveThisMatch.Battle
{
    public class UIEngraveCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Toggles
        {
            EngraveToggle,
        }
        enum CanvasGroup
        {
            EngravePanel,
        }
        #endregion

        private CanvasGroupController _panel;

        private UIProbabilityTable _uiProbabilityTable;
        private UIEngraveButton[] _uiEngraveButtons;

        protected override void Initialize()
        {
            _uiProbabilityTable = GetComponentInChildren<UIProbabilityTable>();
            _uiEngraveButtons = GetComponentsInChildren<UIEngraveButton>();

            BindToggle(typeof(Toggles));
            BindCanvasGroupController(typeof(CanvasGroup));

            _panel = GetCanvasGroupController((int)CanvasGroup.EngravePanel);

            GetToggle((int)Toggles.EngraveToggle).onValueChanged.AddListener(ActivePanel);
        }

        internal void InitializeBattle()
        {
            foreach (var button in _uiEngraveButtons)
            {
                button.InitializeBattle(this);
            }
        }

        internal void DeinitializeBattle()
        {
            foreach (var button in _uiEngraveButtons)
            {
                button.DeinitializeBattle();
            }
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

        internal void ProbabilityRefrash(AgentRarityProbabilityData data)
        {
            _uiProbabilityTable.Show(data);
        }
    }
}