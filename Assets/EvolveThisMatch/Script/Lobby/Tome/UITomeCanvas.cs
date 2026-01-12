using FrameWork.UIBinding;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UITomeCanvas : UIBase
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
            var listCanvas = GetComponentInChildren<UITomeListCanvas>();
            var infoView = GetComponentInChildren<UITomeInfoView>();
            var equipView = GetComponentInChildren<UITomeEquipView>();

            var model = new UITomeModel();
            var presenter = new UITomePresenter(listCanvas, infoView, equipView, model);

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