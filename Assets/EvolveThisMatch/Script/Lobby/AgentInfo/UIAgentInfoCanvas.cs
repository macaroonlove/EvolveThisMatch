using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentInfoCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            CloseButton,
        }
        #endregion

        private UIAgentListCanvas _agentListCanvas;
        private UIAgentDetailCanvas _agentDetailCanvas;

        protected override void Initialize()
        {
            _agentListCanvas = GetComponentInChildren<UIAgentListCanvas>();
            _agentDetailCanvas = GetComponentInChildren<UIAgentDetailCanvas>();

            _agentListCanvas.Initialize((AgentTemplate template, ProfileSaveData.Agent owned) => _agentDetailCanvas.Show(template, owned));
            _agentDetailCanvas.Initialize(_agentListCanvas.RegistAgentListItem);

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
        }
    }
}