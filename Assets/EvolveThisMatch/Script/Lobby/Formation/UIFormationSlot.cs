using FrameWork;
using FrameWork.Editor;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UIFormationSlot : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            UIFormationSlot,
        }
        enum Images
        {
            FaceImage,
            LEIcon,
        }
        enum Texts
        {
            DisplayName,
        }
        enum CanvasGroupControllers
        {
            Active,
        }
        #endregion

        [SerializeField, Label("잠금 이미지")] private Sprite _lockSprite;
        [SerializeField, Label("빈 이미지")] private Sprite _emptySprite;

        private AgentTemplate _template;
        private UnityAction _onChangeSlot;

        private Image _faceImage;
        private Image _leIcon;
        private TextMeshProUGUI _displayName;
        private CanvasGroupController _activeController;

        internal bool isEmpty => _template == null;
        internal AgentTemplate template => _template;

        protected override void Initialize()
        {
            BindButton(typeof(Buttons));
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindCanvasGroupController(typeof(CanvasGroupControllers));

            _faceImage = GetImage((int)Images.FaceImage);
            _leIcon = GetImage((int)Images.LEIcon);
            _displayName = GetText((int)Texts.DisplayName);
            _activeController = GetCanvasGroupController((int)CanvasGroupControllers.Active);
            GetButton((int)Buttons.UIFormationSlot).onClick.AddListener(OnClick);
        }

        internal void Show(AgentTemplate template, UnityAction onChangeSlot)
        {
            _template = template;
            _onChangeSlot = onChangeSlot;

            Apply();
        }

        internal void Lock()
        {
            Apply(true);
        }

        internal void Change(AgentTemplate template)
        {
            _template = template;

            Apply();
        }

        internal void Clear()
        {
            _template = null;

            Apply();
        }

        private void OnClick()
        {
            _onChangeSlot?.Invoke();
        }

        private void Apply(bool isLock = false)
        {
            if (isLock)
            {
                _leIcon.sprite = _lockSprite;
                _activeController.Hide(true);
            }
            else if (isEmpty)
            {
                _leIcon.sprite = _emptySprite;
                _activeController.Hide(true);
            }
            else
            {
                _faceImage.sprite = _template.sprite;
                _displayName.text = _template.displayName;
                _activeController.Show(true);
            }
        }
    }
}