using FrameWork.UIBinding;
using PlayFab;
using TMPro;
using UnityEngine.UI;

namespace FrameWork.PlayFabExtensions
{
    public class UIPlayFabSignupPopup : UIBase
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
            PasswordRepeatInputField,
        }
        enum Buttons
        {
            SignupButton,
            CancelButton,
        }
        #endregion

        private TextMeshProUGUI _errorText;
        private TMP_InputField _idInputField;
        private TMP_InputField _passwordInputField;
        private TMP_InputField _passwordRepeatInputField;
        private Button _signupButton;
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

            _errorText = GetText((int)Texts.ErrorText);
            _idInputField = GetInputField((int)InputFields.IDInputField);
            _passwordInputField = GetInputField((int)InputFields.PasswordInputField);
            _passwordRepeatInputField = GetInputField((int)InputFields.PasswordRepeatInputField);
            _signupButton = GetButton((int)Buttons.SignupButton);

            _signupButton.onClick.AddListener(Signup);
            GetButton((int)Buttons.CancelButton).onClick.AddListener(Cancel);
        }

        private void Signup()
        {
            var id = _idInputField.text;
            var password = _passwordInputField.text;
            var passwordRepeat = _passwordRepeatInputField.text;

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
            if (password != passwordRepeat)
            {
                _errorText.text = "비밀번호가 일치하지 않습니다.";
                return;
            }

            _signupButton.interactable = false;
            PlayFabAuthService.Instance.Email = id;
            PlayFabAuthService.Instance.Password = password;
            PlayFabAuthService.Instance.Authenticate(Authtypes.CraeteAccount);
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
                    _errorText.text = "회원가입을 할 수 없습니다.\n인터넷 등을 확인하세요.";
                    break;
            }

            _signupButton.interactable = true;
        }

        private void Cancel()
        {
            _popupCanvas.ShowLoginPopup();

            _idInputField.text = "";
            _passwordInputField.text = "";
            _passwordRepeatInputField.text = "";
            _errorText.text = "";
        }
    }
}