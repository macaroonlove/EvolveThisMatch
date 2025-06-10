using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

namespace FrameWork.GameSettings
{
    /// <summary>
    /// 단일 액션을 리바인딩하기 위한 독립적인 UI를 갖춘 재사용 가능한 컴포넌트입니다.
    /// </summary>
    public class RebindActionUI : MonoBehaviour
    {
        #region 프로퍼티
        /// <summary>
        /// 리바인딩할 액션에 대한 참조
        /// </summary>
        public InputActionReference actionReference
        {
            get => m_Action;
            set
            {
                m_Action = value;
                UpdateActionLabel();
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// 액션에서 리바인딩할 바인딩의 ID(문자열 형식)
        /// </summary>
        public string bindingId
        {
            get => m_BindingId;
            set
            {
                m_BindingId = value;
                UpdateBindingDisplay();
            }
        }

        public InputBinding.DisplayStringOptions displayStringOptions
        {
            get => m_DisplayStringOptions;
            set
            {
                m_DisplayStringOptions = value;
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// 액션의 이름을 표시하는 텍스트 컴포넌트
        /// </summary>
        public TextMeshProUGUI actionLabel
        {
            get => m_ActionLabel;
            set
            {
                m_ActionLabel = value;
                UpdateActionLabel();
            }
        }

        /// <summary>
        /// 엑션의 path를 표시하는 텍스트 컴포넌트
        /// </summary>
        public TextMeshProUGUI pathLabel
        {
            get => m_PathLabel;
            set
            {
                m_PathLabel = value;
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// 리바인딩 패널에 보여질 선택된 엑션의 이름을 표시하는 텍스트 컴포넌트
        /// </summary>
        public TextMeshProUGUI rebindActionLabel
        {
            get => m_RebindActionLabel;
            set => m_RebindActionLabel = value;
        }

        /// <summary>
        /// 리바인딩 패널에 보여질 선택된 엑션의 path를 표시하는 텍스트 컴포넌트
        /// </summary>
        public TextMeshProUGUI rebindPathLabel
        {
            get => m_RebindPathLabel;
            set => m_RebindPathLabel = value;
        }

        /// <summary>
        /// 리바인딩이 진행중일 때 표시될 UI 패널
        /// </summary>
        public CanvasGroupController rebindOverlay
        {
            get => m_RebindOverlay;
            set => m_RebindOverlay = value;
        }

        /// <summary>
        /// UI가 현재 바인딩을 반영하도록 업데이트될 때마다 트리거되는 이벤트입니다. 사용자 정의 시각화를 바인딩에 연결하는 데 사용할 수 있습니다.
        /// </summary>
        public UpdateBindingUIEvent updateBindingUIEvent
        {
            get
            {
                if (m_UpdateBindingUIEvent == null)
                    m_UpdateBindingUIEvent = new UpdateBindingUIEvent();
                return m_UpdateBindingUIEvent;
            }
        }

        /// <summary>
        /// 액션에서 인터랙티브 리바인딩이 시작될 때 트리거되는 이벤트입니다.
        /// </summary>
        public InteractiveRebindEvent startRebindEvent
        {
            get
            {
                if (m_RebindStartEvent == null)
                    m_RebindStartEvent = new InteractiveRebindEvent();
                return m_RebindStartEvent;
            }
        }

        /// <summary>
        /// 인터랙티브 리바인딩이 완료되거나 취소될 때 트리거되는 이벤트입니다.
        /// </summary>
        public InteractiveRebindEvent stopRebindEvent
        {
            get
            {
                if (m_RebindStopEvent == null)
                    m_RebindStopEvent = new InteractiveRebindEvent();
                return m_RebindStopEvent;
            }
        }

        /// <summary>
        /// 인터랙티브 리바인딩이 진행 중일 때, 이는 리바인딩 작업 컨트롤러입니다. 그렇지 않으면 <c>null</c>입니다.
        /// </summary>
        public InputActionRebindingExtensions.RebindingOperation ongoingRebind => m_RebindOperation;
        #endregion

        /// <summary>
        /// 컴포넌트가 대상으로 하는 바인딩의 액션과 바인딩 인덱스를 반환합니다.
        /// </summary>
        public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;

            action = m_Action?.action;
            if (action == null)
                return false;

            if (string.IsNullOrEmpty(m_BindingId))
                return false;

            // 바인딩 인덱스 조회
            var bindingId = new Guid(m_BindingId);
            bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
            if (bindingIndex == -1)
            {
                Debug.LogError($"ID '{bindingId}'를 가진 바인딩을 '{action}'에서 찾을 수 없습니다.", this);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 현재 표시된 바인딩을 새로 고칩니다.
        /// </summary>
        public void UpdateBindingDisplay()
        {
            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);

            // 액션에서 표시 문자열 가져오기
            var action = m_Action?.action;
            if (action != null)
            {
                var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
                if (bindingIndex != -1)
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, displayStringOptions);
            }

            // 레이블에 설정 (있다면)
            if (m_PathLabel != null)
                m_PathLabel.text = displayString;

            // 리스너에게 UI를 구성할 기회 제공
            m_UpdateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
        }

        /// <summary>
        /// 현재 적용된 바인딩 오버라이드를 제거합니다.
        /// </summary>
        public void ResetToDefault()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            if (action.bindings[bindingIndex].isComposite)
            {
                for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                    action.RemoveBindingOverride(i);
            }
            else
            {
                action.RemoveBindingOverride(bindingIndex);
            }
            UpdateBindingDisplay();
        }

        /// <summary>
        /// 플레이어가 제어를 활성화하여 액션에 대한 새 바인딩을 선택할 수 있도록 하는 인터랙티브 리바인딩을 시작합니다.
        /// </summary>
        public void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            // 바인딩이 컴포지트인 경우 각 부분을 차례로 재바인딩해야 합니다.
            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
            }
            else
            {
                PerformInteractiveRebind(action, bindingIndex);
            }
        }

        private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            m_RebindOperation?.Cancel();

            void CleanUp()
            {
                m_RebindOperation?.Dispose();
                m_RebindOperation = null;
            }

            // 사용 전에 액션 비활성화
            action.Disable();
            // 리바인딩 구성
            // WithControlsExcluding: 해당 path로는 변경 불가
            // WithCancelingThrough: 해당 path는 넘기기
            m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("<Mouse>/leftButton")
                .WithControlsExcluding("<Mouse>/rightButton")
                .WithControlsExcluding("<Mouse>/middleButton")
                .WithControlsExcluding("<Mouse>/press")
                .WithControlsExcluding("<Mouse>/scroll")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnCancel(
                    operation =>
                    {
                        action.Enable();
                        m_RebindStopEvent?.Invoke(this, operation);
                        m_RebindOverlay?.Hide(true);
                        UpdateBindingDisplay();
                        CleanUp();
                    })
                .OnComplete(
                    operation =>
                    {
                        action.Enable();
                        m_RebindOverlay?.Hide(true);
                        m_RebindStopEvent?.Invoke(this, operation);

                        if (CheckDuplicateBindings(action, bindingIndex, allCompositeParts))
                        {
                            action.RemoveBindingOverride(bindingIndex);
                            CleanUp();
                            PerformInteractiveRebind(action, bindingIndex, allCompositeParts);
                            return;
                        }

                        UpdateBindingDisplay();
                        CleanUp();

                        // 더 많은 컴포지트 부분이 있다면, 다음 부분에 대해 리바인딩을 시작합니다.
                        if (allCompositeParts)
                        {
                            var nextBindingIndex = bindingIndex + 1;
                            if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                                PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                    });

            // 바인딩이 부분 바인딩인 경우, UI에 부분 이름을 표시합니다.
            string act = action.bindings[bindingIndex].action;
            var currentLanguage = LocalizationSettings.SelectedLocale;
            string partName = LocalizationSettings.StringDatabase.GetLocalizedString("SettingTable", act, currentLanguage);

            string path = action.GetBindingDisplayString(bindingIndex, displayStringOptions);

            m_RebindOverlay?.Show(true);
            if (m_RebindActionLabel != null)
            {
                m_RebindActionLabel.text = partName;
            }
            if (m_RebindPathLabel != null)
            {
                m_RebindPathLabel.text = path;
            }

            // 리바인딩 패널이 없는 경우, path 라벨을 "<기다리는 중...>"으로 설정합니다.
            if (m_RebindOverlay == null && m_PathLabel != null)
                m_PathLabel.text = "<기다리는 중...>";

            // 리스너에게 리바인딩 시작에 대한 조치를 취할 기회를 제공합니다.
            m_RebindStartEvent?.Invoke(this, m_RebindOperation);

            m_RebindOperation.Start();
        }

        private bool CheckDuplicateBindings(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            InputBinding newBinding = action.bindings[bindingIndex];
            for (int i = 0; i < action.actionMap.bindings.Count; i++)
            {
                if (action.actionMap.bindings[i].action.Equals(newBinding.action))
                {
                    if (newBinding.action != "Move")
                        continue;
                    else
                    {
                        if (action.actionMap.bindings[i].name.Equals(newBinding.name)) continue;
                    }
                }
                if (action.actionMap.bindings[i].effectivePath.Equals(newBinding.effectivePath))
                {
                    return true;
                }
            }
            if (allCompositeParts)
            {
                for (int i = 1; i < bindingIndex; i++)
                {
                    if (action.bindings[i].effectivePath.Equals(newBinding.effectivePath))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected void OnEnable()
        {
            if (s_RebindActionUIs == null)
                s_RebindActionUIs = new List<RebindActionUI>();
            s_RebindActionUIs.Add(this);
            if (s_RebindActionUIs.Count == 1)
                InputSystem.onActionChange += OnActionChange;
        }

        protected void OnDisable()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;

            s_RebindActionUIs.Remove(this);
            if (s_RebindActionUIs.Count == 0)
            {
                s_RebindActionUIs = null;
                InputSystem.onActionChange -= OnActionChange;
            }
        }

        // 액션 시스템이 바인딩을 다시 해결할 때, UI를 업데이트합니다.
        // 우리가 한 변경 사항에서 발생하는 트리거뿐만 아니라 다른 곳에서 한 변경 사항에도 반응합니다.
        // 예를 들어, 사용자가 키보드 레이아웃을 변경하면 바운드 컨트롤 변경 알림을 받고 현재 키보드 레이아웃을 반영하도록 UI를 업데이트합니다.
        private static void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged)
                return;

            var action = obj as InputAction;
            var actionMap = action?.actionMap ?? obj as InputActionMap;
            var actionAsset = actionMap?.asset ?? obj as InputActionAsset;

            for (var i = 0; i < s_RebindActionUIs.Count; ++i)
            {
                var component = s_RebindActionUIs[i];
                var referencedAction = component.actionReference?.action;
                if (referencedAction == null)
                    continue;

                if (referencedAction == action ||
                    referencedAction.actionMap == actionMap ||
                    referencedAction.actionMap?.asset == actionAsset)
                    component.UpdateBindingDisplay();
            }
        }

        [Tooltip("UI에서 재바인딩할 액션에 대한 참조입니다.")]
        [SerializeField]
        private InputActionReference m_Action;

        [SerializeField]
        private string m_BindingId;

        [SerializeField]
        private InputBinding.DisplayStringOptions m_DisplayStringOptions;

        [Tooltip("현재 바인딩한 Action의 표시 이름")]
        [SerializeField]
        private TextMeshProUGUI m_ActionLabel;

        [Tooltip("현재 바인딩한 Action의 Path")]
        [SerializeField]
        private TextMeshProUGUI m_PathLabel;

        [Tooltip("리바인딩이 진행 중일 때 표시될 UI 패널")]
        [SerializeField]
        private CanvasGroupController m_RebindOverlay;

        [Tooltip("리바인딩 패널에 보여질 선택된 Action의 표시 이름")]
        [SerializeField]
        private TextMeshProUGUI m_RebindActionLabel;
        
        [Tooltip("리바인딩 패널에 보여질 선택된 Action의 path")]
        [SerializeField]
        private TextMeshProUGUI m_RebindPathLabel;

        [Tooltip("바인딩 표시 방식이 업데이트될 때 트리거되는 이벤트")]
        [SerializeField]
        private UpdateBindingUIEvent m_UpdateBindingUIEvent;

        [Tooltip("리바인딩이 시작될 때 트리거되는 이벤트")]
        [SerializeField]
        private InteractiveRebindEvent m_RebindStartEvent;

        [Tooltip("리바인딩이 완료되거나 취소될 때 트리거되는 이벤트")]
        [SerializeField]
        private InteractiveRebindEvent m_RebindStopEvent;

        private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;

        private static List<RebindActionUI> s_RebindActionUIs;

        // 에디터에서도 액션 이름 레이블을 업데이트
#if UNITY_EDITOR
        protected void OnValidate()
        {
            UpdateActionLabel();
            UpdateBindingDisplay();
        }

#endif

        private void UpdateActionLabel()
        {
            if (m_ActionLabel != null)
            {
                var action = m_Action?.action;

                if (action != null)
                {
                    switch (action.name)
                    {
                        case "Move":
                            m_ActionLabel.text = "이동(상좌하우 순서로 설정해주세요)";
                            break;
                        case "Sprint":
                            m_ActionLabel.text = "달리기";
                            break;
                        case "Jump":
                            m_ActionLabel.text = "점프/매달리기";
                            break;
                        case "Crouch":
                            m_ActionLabel.text = "앉기";
                            break;
                        case "Interaction":
                            m_ActionLabel.text = "상호작용";
                            break;
                        default:
                            m_ActionLabel.text = string.Empty;
                            break;
                    }
                }
            }
        }

        [Serializable]
        public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, string, string, string>
        {
        }

        [Serializable]
        public class InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation>
        {
        }
    }
}
