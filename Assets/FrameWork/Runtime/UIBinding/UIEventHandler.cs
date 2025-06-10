using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FrameWork.UIBinding
{
    public enum EUIEvent
    {
        Pressed,
        PointerDown,
        PointerUp,
        Hover,
        Exit,
        Drag,
        BeginDrag,
        EndDrag,
        Scroll,
        Select,
        Deselect
    }

    public class UIEventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler, ISelectHandler, IDeselectHandler
    {
        public UnityAction onPointerPressedHandler = null;
        public UnityAction onPointerDownHandler = null;
        public UnityAction onPointerUpHandler = null;
        public UnityAction<BaseEventData> onHoverHandler = null;
        public UnityAction<BaseEventData> onExitHandler = null;
        public UnityAction<BaseEventData> onDragHandler = null;
        public UnityAction<BaseEventData> onBeginDragHandler = null;
        public UnityAction<BaseEventData> onEndDragHandler = null;
        public UnityAction<BaseEventData> onScrollHandler = null;
        public UnityAction<BaseEventData> onSelectHandler = null;
        public UnityAction<BaseEventData> onDeselectHandler = null;

        private bool pressed = false;

        private void Update()
        {
            if (pressed)
                onPointerPressedHandler?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pressed = true;
            onPointerDownHandler?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pressed = false;
            onPointerUpHandler?.Invoke();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            onHoverHandler?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onExitHandler?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDragHandler?.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDragHandler?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDragHandler?.Invoke(eventData);
        }

        public void OnScroll(PointerEventData eventData)
        {
            onScrollHandler?.Invoke(eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            onSelectHandler?.Invoke(eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            onDeselectHandler?.Invoke(eventData);
        }
    }
}