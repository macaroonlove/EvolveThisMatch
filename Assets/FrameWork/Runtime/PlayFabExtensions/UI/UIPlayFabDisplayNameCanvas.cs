using FrameWork.UIBinding;
using TMPro;
using UnityEngine.Events;

namespace FrameWork.PlayFabExtensions
{
    public class UIPlayFabDisplayNameCanvas : UIBase
    {
        #region 바인딩
        enum Texts
        {
            ErrorText,
        }
        enum InputFields
        {
            DisplayNameInputField,
        }
        enum Buttons
        {
            ConfirmButton,
            CancelButton,
        }
        #endregion

        private TextMeshProUGUI _errorText;
        private TMP_InputField _displayNameInputField;

        private UnityAction<string> _onConfirm;

        public void Show(UnityAction<string> confirm)
        {
            base.Show(true);

            _onConfirm = confirm;
        }

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindInputField(typeof(InputFields));
            BindButton(typeof(Buttons));

            _errorText = GetText((int)Texts.ErrorText);
            _displayNameInputField = GetInputField((int)InputFields.DisplayNameInputField);
            GetButton((int)Buttons.ConfirmButton).onClick.AddListener(Confirm);
            GetButton((int)Buttons.CancelButton).onClick.AddListener(Cancel);
        }

        private void Confirm()
        {
            var displayName = _displayNameInputField.text;

            if (string.IsNullOrEmpty(displayName))
            {
                _errorText.text = "이름을 입력해주세요.";
                return;
            }
            else if (displayName.Length < 3 || displayName.Length > 18)
            {
                _errorText.text = "허용된 길이(3~18자 사이)를 벗어났습니다.";
                return;
            }

            _onConfirm?.Invoke(displayName);
            
            Cancel();
        }

        private void Cancel()
        {
            _onConfirm = null;
            Hide(true);
        }
    }
}