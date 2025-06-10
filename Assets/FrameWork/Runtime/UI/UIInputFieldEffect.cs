using DG.Tweening;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;

namespace FrameWork.UI
{
    public class UIInputFieldEffect : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Placeholder,
        }
        #endregion

        private TMP_InputField _inputField;
        private TextMeshProUGUI _placeholder;

        [SerializeField] private Vector2 _targetPosition;
        [SerializeField] private float _targetScale;
        private Vector2 _originalPosition;
        private Vector3 _originalScale;

        protected override void Initialize()
        {
            BindText(typeof(Texts));

            _inputField = GetComponent<TMP_InputField>();
            _placeholder = GetText((int)Texts.Placeholder);

            _originalPosition = _placeholder.rectTransform.anchoredPosition;
            _originalScale = _placeholder.rectTransform.localScale;

            _inputField.onSelect.AddListener(OnInputSelected);
            _inputField.onDeselect.AddListener(OnInputDeselected);
        }

        private void OnInputSelected(string text)
        {
            AnimatePlaceholder(_targetPosition, _targetScale);
        }

        private void OnInputDeselected(string text)
        {
            if (string.IsNullOrEmpty(_inputField.text))
            {
                AnimatePlaceholder(_originalPosition, _originalScale.x);
            }
        }

        private void AnimatePlaceholder(Vector2 position, float scale)
        {
            _placeholder.rectTransform.DOLocalMove(position, 0.4f);
            _placeholder.rectTransform.DOScale(scale, 0.4f);
        }
    }
}