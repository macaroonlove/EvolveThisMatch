using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameWork.UIPopup
{
    public class UIConfirmPopup : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Text,
        }
        enum Buttons
        {
            Button,
        }
        #endregion

        public event UnityAction OnResult;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            GetButton((int)Buttons.Button).onClick.AddListener(() => Hide());
        }

        public void Show(string context)
        {
            GetText((int)Texts.Text).text = context;

            base.Show();
        }

        public override void Hide(bool isForce = false)
        {
            base.Hide(isForce);

            OnResult?.Invoke();
            OnResult = null;
        }
    }
}