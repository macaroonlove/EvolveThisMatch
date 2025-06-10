using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UIBinding
{
    [RequireComponent(typeof(CanvasGroupController))]
    public class UIBase : MonoBehaviour
    {
        private CanvasGroupController _controller;
        private Dictionary<Type, UnityEngine.Object[]> _objects = new();

        #region 예시

        #region UI 요소
        //enum Objects
        //{

        //}
        //enum Images
        //{

        //}
        //enum Texts
        //{

        //}
        //enum InputFields
        //{

        //}
        //enum Buttons
        //{

        //}
        //enum Toggles
        //{

        //}
        //enum Sliders
        //{

        //}
        //enum Dropdowns
        //{

        //}
        //enum CanvasGroupControllers
        //{

        //}
        #endregion

        /// <summary>
        /// 바인딩 방법 참고용
        /// </summary>
        protected virtual void Initialize()
        {
            //BindObject(typeof(Objects));
            //BindImage(typeof(Images));
            //BindText(typeof(Texts));
            //BindInputField(typeof(InputFields));
            //BindButton(typeof(Buttons));
            //BindToggle(typeof(Toggles));
            //BindSlider(typeof(Sliders));
            //BindDropdown(typeof(Dropdowns));
            //BindCanvasGroupController(typeof(CanvasGroupControllers));
        }
        #endregion

        #region 캔버스 그룹 조작하기
        protected virtual void Awake()
        {
            _controller = GetComponent<CanvasGroupController>();

            Initialize();
        }

        public virtual void Show(bool isForce = false)
        {
            if (_controller == null)
            {
                _controller = GetComponent<CanvasGroupController>();
            }

            _controller.Show(isForce);
        }

        public virtual void Hide(bool isForce = false)
        {
            if (_controller == null)
            {
                _controller = GetComponent<CanvasGroupController>();
            }

            _controller.Hide(isForce);
        }
        #endregion

        #region 바인딩
        private void Bind<T>(Type type) where T : UnityEngine.Object
        {
            string[] names = Enum.GetNames(type);
            var objs = new UnityEngine.Object[names.Length];
            _objects.Add(typeof(T), objs);

            for (int i = 0; i < names.Length; i++)
            {
                if (typeof(T) == typeof(GameObject))
                    objs[i] = gameObject.FindChild(names[i], true);
                else
                    objs[i] = gameObject.FindChild<T>(names[i], true);

#if UNITY_EDITOR
                if (objs[i] == null)
                    Debug.LogError($"{name} 오브젝트에서 {names[i]} 바인딩을 실패하였습니다.");
#endif
            }
        }

        protected void BindObject(Type type) => Bind<GameObject>(type);
        protected void BindImage(Type type) => Bind<Image>(type);
        protected void BindText(Type type) => Bind<TextMeshProUGUI>(type);
        protected void BindInputField(Type type) => Bind<TMP_InputField>(type);
        protected void BindButton(Type type) => Bind<Button>(type);
        protected void BindToggle(Type type) => Bind<Toggle>(type);
        protected void BindSlider(Type type) => Bind<Slider>(type);
        protected void BindDropdown(Type type) => Bind<TMP_Dropdown>(type);
        protected void BindCanvasGroupController(Type type) => Bind<CanvasGroupController>(type);
        #endregion

        #region 객체 가져오기
        private T Get<T>(int idx) where T : UnityEngine.Object
        {
            if (!_objects.TryGetValue(typeof(T), out var objs))
                return null;
            return objs[idx] as T;
        }

        protected GameObject GetObject(int idx) => Get<GameObject>(idx);
        protected Image GetImage(int idx) => Get<Image>(idx);
        protected TextMeshProUGUI GetText(int idx) => Get<TextMeshProUGUI>(idx);
        protected TMP_InputField GetInputField(int idx) => Get<TMP_InputField>(idx);
        protected Button GetButton(int idx) => Get<Button>(idx);
        protected Toggle GetToggle(int idx) => Get<Toggle>(idx);
        protected Slider GetSlider(int idx) => Get<Slider>(idx);
        protected TMP_Dropdown GetDropdown(int idx) => Get<TMP_Dropdown>(idx);
        protected CanvasGroupController GetCanvasGroupController(int idx) => Get<CanvasGroupController>(idx);
        #endregion

        #region 이벤트 등록
        /// <summary>
        /// 이벤트 등록하기
        /// </summary>
        internal static void BindEvent(GameObject obj, UnityAction action = null, UnityAction<BaseEventData> dataAction = null, EUIEvent type = EUIEvent.Pressed)
        {
            UIEventHandler uIEventHandler = obj.GetOrAddComponent<UIEventHandler>();

            switch (type)
            {
                case EUIEvent.Pressed:
                    uIEventHandler.onPointerPressedHandler -= action;
                    uIEventHandler.onPointerPressedHandler += action;
                    break;
                case EUIEvent.PointerUp:
                    uIEventHandler.onPointerUpHandler -= action;
                    uIEventHandler.onPointerUpHandler += action;
                    break;
                case EUIEvent.PointerDown:
                    uIEventHandler.onPointerDownHandler -= action;
                    uIEventHandler.onPointerDownHandler += action;
                    break;
                case EUIEvent.Hover:
                    uIEventHandler.onHoverHandler -= dataAction;
                    uIEventHandler.onHoverHandler += dataAction;
                    break;
                case EUIEvent.Exit:
                    uIEventHandler.onExitHandler -= dataAction;
                    uIEventHandler.onExitHandler += dataAction;
                    break;
                case EUIEvent.Drag:
                    uIEventHandler.onDragHandler -= dataAction;
                    uIEventHandler.onDragHandler += dataAction;
                    break;
                case EUIEvent.BeginDrag:
                    uIEventHandler.onBeginDragHandler -= dataAction;
                    uIEventHandler.onBeginDragHandler += dataAction;
                    break;
                case EUIEvent.EndDrag:
                    uIEventHandler.onEndDragHandler -= dataAction;
                    uIEventHandler.onEndDragHandler += dataAction;
                    break;
                case EUIEvent.Scroll:
                    uIEventHandler.onScrollHandler -= dataAction;
                    uIEventHandler.onScrollHandler += dataAction;
                    break;
                case EUIEvent.Select:
                    uIEventHandler.onSelectHandler -= dataAction;
                    uIEventHandler.onSelectHandler += dataAction;
                    break;
                case EUIEvent.Deselect:
                    uIEventHandler.onDeselectHandler -= dataAction;
                    uIEventHandler.onDeselectHandler += dataAction;
                    break;
            }
        }
        #endregion
    }
}