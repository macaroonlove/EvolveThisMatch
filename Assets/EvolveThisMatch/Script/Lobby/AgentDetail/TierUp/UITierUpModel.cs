using EvolveThisMatch.Save;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public sealed class UITierUpModel
    {
        private readonly AgentSaveDataTemplate _agentData;
        private AgentSaveData.Agent _owned;

        public UITierUpModel()
        {
            _agentData = SaveManager.Instance.agentData;
        }

        public void Bind(AgentSaveData.Agent owned)
        {
            _owned = owned;
        }

        public bool IsEmpty => _owned == null;

        public TierUpViewState BuildViewState()
        {
            if (_owned == null)
                return default;

            int tier = _owned.tier;
            int unitCount = _owned.unitCount;
            int maxUnitCount = _agentData.GetMaxUnitCountByTier(tier);
            bool isMaxTier = maxUnitCount == -1;

            var advantages = new bool[5];
            for (int i = 0; i < advantages.Length; i++)
                advantages[i] = i < tier;

            return new TierUpViewState
            {
                counterText = isMaxTier ? $"{unitCount}" : $"{unitCount}/{maxUnitCount}",
                counterFill = isMaxTier ? 1f : (float)unitCount / maxUnitCount,
                showNextTier = !isMaxTier,
                currentTier = tier,
                nextTier = tier + 1,
                tierAdvantages = advantages,
                canTierUp = _agentData.GetTierUpAbleUnit(_owned.id)
            };
        }

        public void TierUp(UnityAction onComplete)
        {
            if (_owned == null) return;

            _agentData.TierUpAgent(_owned.id, onComplete);
        }
    }
}