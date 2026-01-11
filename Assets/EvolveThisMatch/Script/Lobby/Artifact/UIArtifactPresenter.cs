using EvolveThisMatch.Core;
using EvolveThisMatch.Save;

namespace EvolveThisMatch.Lobby
{
    public sealed class UIArtifactPresenter
    {
        private readonly UIArtifactInfoView _infoView;
        private readonly UIArtifactModel _model;

        public UIArtifactPresenter(UIArtifactListCanvas listCanvas, UIArtifactInfoView infoView, UIArtifactModel model)
        {
            _infoView = infoView;
            _model = model;

            listCanvas.onSelected += SelectItem;
        }

        private void SelectItem(ArtifactTemplate template, ItemSaveData.Artifact owned)
        {
            var state = _model.BuildInfoViewState(template, owned);

            _infoView.Show(state);
        }
    }
}