using FrameWork.UIBinding;
using UnityEngine.Events;

namespace FrameWork.UIPopup
{
    public class UIInputPopup : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Text,
        }
        enum InputFields
        {
            InputField,
        }
        enum Buttons
        {
            Button_Confirm,
            Button_Cancel,
        }
        #endregion

        public event UnityAction<string> OnResult;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindInputField(typeof(InputFields));
            BindButton(typeof(Buttons));

            GetButton((int)Buttons.Button_Confirm).onClick.AddListener(OnConfirm);
            GetButton((int)Buttons.Button_Cancel).onClick.AddListener(OnCancel);
        }

        public void Show(string context)
        {
            GetText((int)Texts.Text).text = context;

            base.Show();
        }

        private void OnConfirm()
        {
            Hide();

            OnResult?.Invoke(GetInputField((int)InputFields.InputField).text);
            OnResult = null;
        }

        private void OnCancel()
        {
            Hide();

            OnResult = null;
        }
    }
}