using FrameWork.UIBinding;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UIAgentSelectItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            Button,
        }
        enum Images
        {
            FaceImage,
            SelectedImage,
        }
        #endregion

        private Image _faceImage;
        private Image _selectedImage;

        protected AgentTemplate _template;
        internal AgentTemplate template => _template;

        private UnityAction<UIAgentSelectItem> _onSelect;

        protected override void Initialize()
        {
            BindButton(typeof(Buttons));
            BindImage(typeof(Images));

            _faceImage = GetImage((int)Images.FaceImage);
            _selectedImage = GetImage((int)Images.SelectedImage);

            GetButton((int)Buttons.Button).onClick.AddListener(OnClick);
        }

        internal void Show(AgentTemplate template, UnityAction<UIAgentSelectItem> onSelect)
        {
            _template = template;
            _onSelect = onSelect;

            _faceImage.sprite = template.sprite;

            gameObject.SetActive(true);
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnClick()
        {
            _onSelect?.Invoke(this);
        }

        internal virtual void Select()
        {
            _selectedImage.enabled = true;
        }

        internal virtual void UnSelect()
        {
            _selectedImage.enabled = false;
        }
    }
}