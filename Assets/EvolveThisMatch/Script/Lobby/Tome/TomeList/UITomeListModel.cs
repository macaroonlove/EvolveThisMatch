using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using System.Collections.Generic;
using System.Linq;

namespace EvolveThisMatch.Lobby
{
    public sealed class UITomeListModel
    {
        private readonly List<TomeTemplate> _templates;
        private readonly Dictionary<int, ItemSaveData.Tome> _owned;

        public int count => _templates.Count;

        public UITomeListModel()
        {
            _templates = GameDataManager.Instance.tomeTemplates.ToList();
            _owned = SaveManager.Instance.itemData.ownedTomes.ToDictionary(a => a.id);
        }

        public TomeTemplate GetTemplate(int index)
        {
            if (index < 0 || index >= _templates.Count) return null;

            return _templates[index];
        }

        public ItemSaveData.Tome GetOwned(TomeTemplate template)
        {
            if (template == null) return null;

            _owned.TryGetValue(template.id, out var owned);
            return owned;
        }

        public TomeListItemViewState BuildState(int index)
        {
            var template = GetTemplate(index);
            if (template == null) return default;

            if (!_owned.TryGetValue(template.id, out var owned)) return default;

            int maxCount = SaveManager.Instance.itemData.GetMaxTomeCountByLevel(owned.level);

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

            return new TomeListItemViewState(true, template.sprite, template.displayName, owned.level, countText, progress);
        }
    }
}