using FrameWork.UIBinding;
using PlayFab.ClientModels;

namespace FrameWork.PlayFabExtensions
{
    public class UIPlayFabPopupCanvas : UIBase
    {
        private UIPlayFabLoginPopup _loginPopup;
        private UIPlayFabSignupPopup _signupPopup;

        protected override void Initialize()
        {
            _loginPopup = GetComponentInChildren<UIPlayFabLoginPopup>();
            _signupPopup = GetComponentInChildren<UIPlayFabSignupPopup>();

            PlayFabAuthService.OnLoginSuccess += OnLoginSuccess;
        }

        private void OnDestroy()
        {
            PlayFabAuthService.OnLoginSuccess -= OnLoginSuccess;
        }

        private void OnLoginSuccess(LoginResult result)
        {
            _loginPopup.Hide(true);
            _signupPopup.Hide(true);
        }

        public override void Show(bool isForce = false)
        {
            _loginPopup.Show(true);
            base.Show(isForce);
        }

        public override void Hide(bool isForce = false)
        {
            _loginPopup.Hide(true);
            _signupPopup.Hide(true);
            base.Hide(isForce);
        }

        internal void ShowLoginPopup()
        {
            _loginPopup.Show(true);
            _signupPopup.Hide(true);
        }

        internal void ShowSignupPopup()
        {
            _loginPopup.Hide(true);
            _signupPopup.Show(true);
        }
    }
}