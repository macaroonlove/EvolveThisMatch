using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIArtifactCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            CloseButton,
        }
        #endregion

        private UIArtifactListCanvas _artifactListCanvas;
        private UIArtifactInfoCanvas _artifactInfoCanvas;
        private UnityAction _onClose;

        protected override void Initialize()
        {
            _artifactListCanvas = GetComponentInChildren<UIArtifactListCanvas>();
            _artifactInfoCanvas = GetComponentInChildren<UIArtifactInfoCanvas>();

            _artifactListCanvas.Initialize((ArtifactTemplate template, ItemSaveData.Artifact owned) => _artifactInfoCanvas.Show(template, owned));
            _artifactInfoCanvas.Initialize(_artifactListCanvas.RegistArtifactListItem);

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
        }

        public void Show(UnityAction onClose)
        {
            _onClose = onClose;

            Show(true);
        }

        private void Hide()
        {
            _onClose?.Invoke();

            Hide(true);
        }
    }
}