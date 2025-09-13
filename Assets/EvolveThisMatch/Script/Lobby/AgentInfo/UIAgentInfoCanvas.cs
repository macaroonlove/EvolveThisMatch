using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UI;
using FrameWork.UIBinding;
using UnityEngine;
using UnityEngine.Events;

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

        private GameObject _overUICamera;
        private UnityAction _onClose;

        protected override void Initialize()
        {
            _agentListCanvas = GetComponentInChildren<UIAgentListCanvas>();
            _agentDetailCanvas = GetComponentInChildren<UIAgentDetailCanvas>();
            _overUICamera = Camera.main.transform.Find("OverUICamera").gameObject;

            _agentListCanvas.Initialize((AgentTemplate template, AgentSaveData.Agent owned) => _agentDetailCanvas.Show(template, owned));
            _agentDetailCanvas.Initialize(_agentListCanvas.RegistAgentListItem);

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
        }

        public void Show(UnityAction onClose)
        {
            _onClose = onClose;

            _agentListCanvas.SelectFirstItem();
            _overUICamera.SetActive(true);

            base.Show(true);
        }

        public void Hide()
        {
            _agentDetailCanvas.HidePanel();
            _overUICamera.SetActive(false);

            _onClose?.Invoke();
            base.Hide(true);
        }
    }
}