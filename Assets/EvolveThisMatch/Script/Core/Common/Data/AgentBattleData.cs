using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AgentBattleData
    {
        private AgentTemplate _agentTemplate;
        private AgentUnit _agentUnit;
        private int _level;
        private AgentRarityTemplate _limit;
        private CoinSystem _coinSystem;
        private CrystalSystem _crystalSystem;

        internal AgentTemplate agentTemplate => _agentTemplate;
        internal AgentUnit agentUnit => _agentUnit;
        internal int level => _level;
        internal AgentRarityTemplate limit => _limit;

        public AgentBattleData(AgentUnit agentUnit, AgentTemplate agentTemplate)
        {
            _agentUnit = agentUnit;
            _agentTemplate = agentTemplate;
            _coinSystem = BattleManager.Instance.GetSubSystem<CoinSystem>();
            _crystalSystem = BattleManager.Instance.GetSubSystem<CrystalSystem>();

            _level = 1;
            _limit = GameDataManager.Instance.GetLimitRarity();
        }

        internal int GetNeedCoinToLevelUp()
        {
            var data = _agentTemplate.rarity.agentLevelLibrary.GetLevelData(_level);

            if (data == null) return -1;

            return data.needCoin;
        }

        internal void LevelUp()
        {
            var data = _agentTemplate.rarity.agentLevelLibrary.GetLevelData(_level);

            if (data == null) return;

            if (_coinSystem.PayCoin(data.needCoin))
            {
                _level++;
            }
        }

        internal void UpgradeLimit()
        {
            if (_crystalSystem.PayCrystal(1))
            {
                _limit = GameDataManager.Instance.GetUpgradeLimitRarity(_limit);
            }
        }
    }
}