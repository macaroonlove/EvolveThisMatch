using FrameWork.UIBinding;
using PlayFab;
using TMPro;
using UnityEngine.UI;

namespace FrameWork.PlayFabExtensions
{
    public class UIPlayFabLoginPopup : UIBase
    {
        #region 바인딩
        enum Texts
        {
            ErrorText,
        }
        enum InputFields
        {
            IDInputField,
            PasswordInputField,
        }
        enum Buttons
        {
            LoginButton,
            SignupButton,
            CancelButton,
        }
        enum Toggles
        {
            AutoLoginToggle,
        }
        #endregion

        private TextMeshProUGUI _errorText;
        private TMP_InputField _idInputField;
        private TMP_InputField _passwordInputField;
        private Button _loginButton;
        private Toggle _autoLoginToggle;
        private UIPlayFabPopupCanvas _popupCanvas;

        public override void Show(bool isForce = false)
        {
            base.Show(isForce);

            PlayFabAuthService.OnPlayFabError += OnLoginFailed;
        }

        public override void Hide(bool isForce = false)
        {
            base.Hide(isForce);

            PlayFabAuthService.OnPlayFabError -= OnLoginFailed;
        }

        protected override void Initialize()
        {
            _popupCanvas = GetComponentInParent<UIPlayFabPopupCanvas>();

            BindText(typeof(Texts));
            BindInputField(typeof(InputFields));
            BindButton(typeof(Buttons));
            BindToggle(typeof(Toggles));

            _errorText = GetText((int)Texts.ErrorText);
            _idInputField = GetInputField((int)InputFields.IDInputField);
            _passwordInputField = GetInputField((int)InputFields.PasswordInputField);
            _loginButton = GetButton((int)Buttons.LoginButton);
            _autoLoginToggle = GetToggle((int)Toggles.AutoLoginToggle);

            _loginButton.onClick.AddListener(Login);
            GetButton((int)Buttons.SignupButton).onClick.AddListener(Signup);
            GetButton((int)Buttons.CancelButton).onClick.AddListener(Cancel);
        }

        private void Login()
        {
            var id = _idInputField.text;
            var password = _passwordInputField.text;

            if (string.IsNullOrEmpty(id))
            {
                _errorText.text = "이메일을 입력해주세요.";
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                _errorText.text = "비밀번호를 입력해주세요.";
                return;
            }

            _loginButton.interactable = false;
            PlayFabAuthService.Instance.Email = id;
            PlayFabAuthService.Instance.Password = password;
            PlayFabAuthService.Instance.RememberMe = _autoLoginToggle.isOn;
            PlayFabAuthService.Instance.Authenticate(Authtypes.EmailAndPassword);
        }

        private void OnLoginFailed(PlayFabError result)
        {
            switch (result.Error)
            {
                case PlayFabErrorCode.InvalidEmailAddress:
                    _errorText.text = "잘못된 이메일 입니다.";
                    break;
                case PlayFabErrorCode.InvalidPassword:
                    _errorText.text = "잘못된 비밀번호 입니다.";
                    break;
                case PlayFabErrorCode.InvalidEmailOrPassword:
                    _errorText.text = "잘못된 이메일 또는 비밀번호 입니다.";
                    break;
                case PlayFabErrorCode.AccountNotFound:
                    _errorText.text = "계정을 찾을 수 없습니다.";
                    break;
                default:
                    _errorText.text = "로그인할 수 없습니다.\n인터넷 등을 확인하세요.";
                    break;
            }

            _loginButton.interactable = true;
        }

        private void Signup()
        {
            _popupCanvas.ShowSignupPopup();
        }

        private void Cancel()
        {
            _popupCanvas.Hide(true);

            _idInputField.text = "";
            _passwordInputField.text = "";
            _errorText.text = "";
        }
    }
}