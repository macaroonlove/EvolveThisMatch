using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UI
{
    public class UIVariableInfoCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            CloseButton,
        }
        #endregion

        [SerializeField] private VariableInfoTemplate _variableInfoTemplate;

        private UIVariableListPanel _variableListPanel;
        private UIVariableInfoPanel _variableInfoPanel;

        protected override void Initialize()
        {
            _variableListPanel = GetComponentInChildren<UIVariableListPanel>();
            _variableInfoPanel = GetComponentInChildren<UIVariableInfoPanel>();

            _variableListPanel.Initialize(_variableInfoTemplate, (VariableInfo info) => _variableInfoPanel.Show(info));

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
        }

        public void Show(ObscuredIntVariable variable)
        {
            _variableListPanel.SelectItem(variable);

            Show(true);
        }

        private void Hide()
        {
            Hide(true);
        }
    }
}