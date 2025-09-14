using CodeStage.AntiCheat.ObscuredTypes;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public class UIIntVariableTextEffect : UIBase, IPointerClickHandler
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Icon,
        }
        enum Texts
        {
            Value,
        }
        #endregion

        [SerializeField] private ObscuredIntVariable _variable;
        [SerializeField] private int _maxLength;
        [SerializeField] private bool _isShowInfo;

        private TextMeshProUGUI _valueText;
        private Image _iconImage;
        private UIIntVariableLogEffect _logEffect;

        internal bool isEmpty => _variable == null;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _valueText = GetText((int)Texts.Value);
            _iconImage = GetImage((int)Images.Icon);

            if (_variable != null)
                _iconImage.sprite = _variable.Icon;

            _logEffect = GetComponentInChildren<UIIntVariableLogEffect>();
        }

        internal void SetVariable(ObscuredIntVariable variable)
        {
            Hide();
            _variable = variable;
            _iconImage.sprite = variable.Icon;
            Show();
        }

        private void Start()
        {
            Show();
        }

        public void Show()
        {
            if (_variable == null) return;

            _variable.AddListener(OnChangeValue);
            _logEffect?.Initialize(_variable.Value);
            Apply(_variable.Value);

            base.Show(true);
        }

        public void Hide()
        {
            if (_variable == null) return;

            _variable.RemoveListener(OnChangeValue);
            _variable = null;
            
            base.Hide(true);
        }

        private void OnChangeValue(ObscuredInt value)
        {
            _logEffect?.OnChangeValue(value);

            Apply(value);
        }

        private void Apply(int value)
        {
            _valueText.text = value.Format(_maxLength);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isShowInfo) return;

            VariableDisplayManager.Instance.ShowInfo(_variable);
        }
    }
}