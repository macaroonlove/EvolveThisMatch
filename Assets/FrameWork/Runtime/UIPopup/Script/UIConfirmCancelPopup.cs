using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameWork.UIPopup
{
    public class UIConfirmCancelPopup : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Text,
        }
        enum Buttons
        {
            Button_Confirm,
            Button_Cancel,
        }
        #endregion

        public event UnityAction<bool> OnResult;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
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

            OnResult?.Invoke(true);
            OnResult = null;
        }

        private void OnCancel()
        {
            Hide();

            OnResult?.Invoke(false);
            OnResult = null;
        }
    }
}