using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentInfoCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
        }
        enum Images
        {
            FullBody,
        }
        #endregion

        private Image _fullBody;
        private UIAgentInfoController _controller;

        protected override void Initialize()
        {
            var agentListCanvas = GetComponentInChildren<UIAgentListCanvas>();
            var agentDetailCanvas = GetComponentInChildren<UIAgentDetailCanvas>();
            var overUICamera = Camera.main.transform.Find("OverUICamera").gameObject;

            BindImage(typeof(Images));
            BindButton(typeof(Buttons));

            _fullBody = GetImage((int)Images.FullBody);
            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);

            _controller = new UIAgentInfoController(this, agentListCanvas, agentDetailCanvas, overUICamera);
        }

        public void Show(UnityAction onClose)
        {
            _controller.Show(onClose);

            base.Show(true);
        }

        public void Render(AgentTemplate template)
        {
            // 유닛 풀 이미지
            _fullBody.sprite = template.sprite;
        }

        public void Hide()
        {
            _controller?.Hide();

            base.Hide(true);
        }
    }
}