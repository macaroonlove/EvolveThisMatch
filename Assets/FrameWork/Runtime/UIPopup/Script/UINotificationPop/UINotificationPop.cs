using DG.Tweening;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UIPopup
{
    public class UINotificationPop : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        { 
            Title,
            Description,
        }
        #endregion

        protected override void Initialize()
        {
            BindText(typeof(Texts));
        }

        public virtual void Initialize(string title, string desciption, ENotificationType notificationType, UnityAction<RectTransform, ENotificationType> action)
        {
            GetText((int)Texts.Title).text = title;
            GetText((int)Texts.Description).text = desciption;

            RectTransform rect = transform.GetChild(0) as RectTransform;
            float moveX = rect.sizeDelta.x;

            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(moveX, 0);

                rect.DOLocalMoveX(-moveX, 0.5f).SetRelative(true).SetUpdate(true).OnComplete(() => {
                    DOVirtual.DelayedCall(2f, () =>
                    {
                        rect.DOLocalMoveX(moveX, 0.5f).SetRelative(true).SetUpdate(true).OnComplete(() => {
                            action?.Invoke(transform as RectTransform, notificationType);
                        });
                    }, true);
                });
            }
        }
    }
}