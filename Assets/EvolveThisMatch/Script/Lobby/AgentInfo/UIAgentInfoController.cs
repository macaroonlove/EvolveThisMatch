using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public sealed class UIAgentInfoController
    {
        private readonly UIAgentInfoCanvas _agentInfoCanvas;
        private readonly UIAgentListCanvas_AgentInfo _agentListCanvas;
        private readonly UIAgentDetailCanvas _agentDetailCanvas;
        private readonly GameObject _overUICamera;

        private UnityAction _onClose;

        public UIAgentInfoController(UIAgentInfoCanvas info,UIAgentListCanvas_AgentInfo list, UIAgentDetailCanvas detail, GameObject overUICamera)
        {
            _agentInfoCanvas = info;
            _agentListCanvas = list;
            _agentDetailCanvas = detail;
            _overUICamera = overUICamera;

            _agentListCanvas.Bind(OnAgentSelected);
        }

        public void Show(UnityAction onClose)
        {
            _agentDetailCanvas.SelectFirstPanel();
            _overUICamera.SetActive(true);
            _onClose = onClose;
        }

        public void Hide()
        {
            _agentDetailCanvas.Hide();
            _overUICamera.SetActive(false);
            _onClose?.Invoke();
        }

        private void OnAgentSelected(AgentTemplate template, AgentSaveData.Agent owned)
        {
            _agentInfoCanvas.Render(template);
            _agentDetailCanvas.Show(template, owned);
        }
    }
}