using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameWork.UIPopup
{
    public class UISpriteNotificationPop : UINotificationPop
    {
        enum Images
        {
            Sprite,
        }

        protected override void Initialize()
        {
            base.Initialize();

            BindImage(typeof(Images));
        }

        public void Initialize(string title, string desciption, Sprite sprite, ENotificationType notificationType, UnityAction<RectTransform, ENotificationType> action)
        {
            GetImage((int)Images.Sprite).sprite = sprite;

            base.Initialize(title, desciption, notificationType, action);
        }
    }
}