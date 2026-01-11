using FrameWork.UIBinding;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIArtifactCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            CloseButton,
        }
        #endregion

        private UnityAction _onClose;

        protected override void Initialize()
        {
            var listCanvas = GetComponentInChildren<UIArtifactListCanvas>();
            var infoView = GetComponentInChildren<UIArtifactInfoView>();
            var model = new UIArtifactModel();
            var presenter = new UIArtifactPresenter(listCanvas, infoView, model);

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
        }

        public void Show(UnityAction onClose)
        {
            _onClose = onClose;

            Show(true);
        }

        private void Hide()
        {
            _onClose?.Invoke();

            Hide(true);
        }
    }
}