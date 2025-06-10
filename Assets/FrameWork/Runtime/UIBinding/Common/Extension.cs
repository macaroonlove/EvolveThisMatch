using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FrameWork.UIBinding
{
    public static class Extension
    {
        public static GameObject FindChild(this GameObject obj, string name, bool recursive)
        {
            return Utils.FindChild(obj, name, recursive);
        }

        public static T FindChild<T>(this GameObject obj, string name, bool recursive) where T : UnityEngine.Object
        {
            return Utils.FindChild<T>(obj, name, recursive);
        }

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            return Utils.GetOrAddComponent<T>(obj);
        }

        #region 이벤트 등록 확장 메서드
        public static void BindPressEvent(this GameObject obj, UnityAction action)
        {
            BindEvent(obj, action, null, EUIEvent.Pressed);
        }

        public static void BindPointerUpEvent(this GameObject obj, UnityAction action)
        {
            BindEvent(obj, action, null, EUIEvent.PointerUp);
        }

        public static void BindPointerDownEvent(this GameObject obj, UnityAction action)
        {
            BindEvent(obj, action, null, EUIEvent.PointerDown);
        }

        public static void BindDragEvent(this GameObject obj, UnityAction<BaseEventData> dragAction)
        {
            BindEvent(obj, null, dragAction, EUIEvent.Drag);
        }

        public static void BindBeginDragEvent(this GameObject obj, UnityAction<BaseEventData> dragAction)
        {
            BindEvent(obj, null, dragAction, EUIEvent.BeginDrag);
        }

        public static void BindEndDragEvent(this GameObject obj, UnityAction<BaseEventData> dragAction)
        {
            BindEvent(obj, null, dragAction, EUIEvent.EndDrag);
        }

        public static void BindHoverEvent(this GameObject obj, UnityAction<BaseEventData> hoverAction)
        {
            BindEvent(obj, null, hoverAction, EUIEvent.Hover);
        }

        public static void BindExitEvent(this GameObject obj, UnityAction<BaseEventData> exitAction)
        {
            BindEvent(obj, null, exitAction, EUIEvent.Exit);
        }

        public static void BindScrollEvent(this GameObject obj, UnityAction<BaseEventData> scrollAction)
        {
            BindEvent(obj, null, scrollAction, EUIEvent.Scroll);
        }

        public static void BindSelectEvent(this GameObject obj, UnityAction<BaseEventData> selectAction)
        {
            BindEvent(obj, null, selectAction, EUIEvent.Select);
        }

        public static void BindDeselectEvent(this GameObject obj, UnityAction<BaseEventData> deselectAction)
        {
            BindEvent(obj, null, deselectAction, EUIEvent.Deselect);
        }

        public static void BindEvent(this GameObject obj, UnityAction action = null, UnityAction<BaseEventData> dataAction = null, EUIEvent type = EUIEvent.Pressed)
        {
            UIBase.BindEvent(obj, action, dataAction, type);
        }
        #endregion
    }
}
