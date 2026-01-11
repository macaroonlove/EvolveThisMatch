using EvolveThisMatch.Core;
using EvolveThisMatch.Save;

namespace EvolveThisMatch.Lobby
{
    public sealed class UIArtifactModel
    {
        private EffectContext _effectContext;

        public UIArtifactModel()
        {
            _effectContext = new EffectContext();
        }

        public ArtifactInfoViewState BuildInfoViewState(ArtifactTemplate template, ItemSaveData.Artifact owned)
        {
            _effectContext.artifactSaveData = owned;

            return new ArtifactInfoViewState
            (
                template.displayName,
                template.description.Replace("{value}", $"{template.GetValue("value", _effectContext)}"),
                template.sprite
            );
        }
    }
}