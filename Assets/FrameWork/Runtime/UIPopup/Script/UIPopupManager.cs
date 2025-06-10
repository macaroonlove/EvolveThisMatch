using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UIPopup
{
    public class UIPopupManager : Singleton<UIPopupManager>
    {
        [SerializeField] private UINotificationPopup _notificationPopup;
        [SerializeField] private UIConfirmPopup _confirmPopup;
        [SerializeField] private UIConfirmCancelPopup _confirmCancelPopup;
        [SerializeField] private UIInputPopup _inputPopup;

        /// <summary>
        /// 우측 상단에서 정보만 보여주는 팝업
        /// </summary>
        public void ShowNotificationPopup(string title, string description, ENotificationType notificationType = ENotificationType.BasicNotificationPop, Sprite sprite = null)
        {
            _notificationPopup.Show(title, description, sprite, notificationType);
        }

        /// <summary>
        /// 확인 버튼만 있는 팝업
        /// </summary>
        public void ShowConfirmPopup(string context, UnityAction action = null)
        {
            _confirmPopup.Show(context);

            _confirmPopup.OnResult += () =>
            {
                action?.Invoke();
            };
        }

        /// <summary>
        /// 확인, 취소 버튼이 있는 팝업
        /// </summary>
        public void ShowConfirmCancelPopup(string context, UnityAction<bool> action = null)
        {
            _confirmCancelPopup.Show(context);

            _confirmCancelPopup.OnResult += (result) =>
            {
                action?.Invoke(result);
            };
        }

        /// <summary>
        /// 인풋 필드가 있는 팝업
        /// </summary>
        public void ShowInputPopup(string context, UnityAction<string> action = null)
        {
            _inputPopup.Show(context);

            _inputPopup.OnResult += (string str) =>
            {
                action?.Invoke(str);
            };
        }
    }
}