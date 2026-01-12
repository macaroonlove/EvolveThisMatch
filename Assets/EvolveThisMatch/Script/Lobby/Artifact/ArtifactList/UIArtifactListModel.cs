using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using System.Collections.Generic;
using System.Linq;

namespace EvolveThisMatch.Lobby
{
    public sealed class UIArtifactListModel
    {
        private readonly List<ArtifactTemplate> _templates;
        private readonly Dictionary<int, ItemSaveData.Artifact> _owned;

        public int Count => _templates.Count;

        public UIArtifactListModel()
        {
            _templates = GameDataManager.Instance.artifactTemplates.ToList();
            _owned = SaveManager.Instance.itemData.ownedArtifacts.ToDictionary(a => a.id);
        }

        public ArtifactTemplate GetTemplate(int index)
        {
            if (index < 0 || index >= _templates.Count) return null;

            return _templates[index];
        }

        public ItemSaveData.Artifact GetOwned(ArtifactTemplate template)
        {
            if (template == null) return null;

            _owned.TryGetValue(template.id, out var owned);
            return owned;
        }

        public ArtifactListItemViewState BuildState(int index)
        {
            var template = GetTemplate(index);
            if (template == null) return default;

            if (!_owned.TryGetValue(template.id, out var owned)) return default;

            int maxCount = SaveManager.Instance.itemData.GetMaxArtifactCountByLevel(owned.level);

            string countText;
            float progress;

            if (maxCount < 0)
            {
                countText = owned.count.ToString();
                progress = 1f;
            }
            else
            {
                countText = $"{owned.count}/{maxCount}";
                progress = owned.count / (float)maxCount;
            }

            return new ArtifactListItemViewState(true, template.sprite, template.displayName, owned.level, countText, progress);
        }
    }
}