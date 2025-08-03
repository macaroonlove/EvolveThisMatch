using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;

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

        protected override void Initialize()
        {
            _artifactListCanvas = GetComponentInChildren<UIArtifactListCanvas>();
            _artifactInfoCanvas = GetComponentInChildren<UIArtifactInfoCanvas>();

            _artifactListCanvas.Initialize((ArtifactTemplate template, ProfileSaveData.Artifact owned) => _artifactInfoCanvas.Show(template, owned));
            _artifactInfoCanvas.Initialize(_artifactListCanvas.RegistArtifactListItem);

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
        }
    }
}