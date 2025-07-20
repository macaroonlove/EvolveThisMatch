using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AgentBattleData
    {
        private AgentChangeSystem _agentChangeSystem;

        public AgentTemplate agentTemplate { get; private set; }
        public AgentUnit agentUnit { get; private set; }
        public SignBoard signBoard { get; private set; }
        public int level { get; private set; }
        public AgentRarityTemplate limit { get; private set; }
        public TileController mountTile { get; private set; }

        public AgentBattleData(AgentUnit agentUnit, AgentTemplate agentTemplate)
        {
            this.agentUnit = agentUnit;
            this.agentTemplate = agentTemplate;

            this.level = 1;
            this.limit = GameDataManager.Instance.GetAgentRandomRarity();

            _agentChangeSystem = BattleManager.Instance.GetSubSystem<AgentChangeSystem>();
        }

        #region 위치
        internal void ComfirmTile(TileController tile)
        {
            mountTile = tile;
        }
        #endregion

        #region 출격 시, 표지판
        internal void RegistSignBoard(SignBoard signBoard)
        {
            this.signBoard = signBoard;
        }

        public void DeregistSignBoard()
        {
            signBoard = null;
        }
        #endregion

        #region 레벨업
        internal int GetNeedCoinToLevelUp()
        {
            var data = agentTemplate.rarity.agentLevelLibrary.GetLevelData(level);

            if (data == null) return -1;

            return data.needCoin;
        }

        internal void LevelUp()
        {
            level++;
        }
        #endregion

        #region 재능 제한
        internal void UpgradeLimit()
        {
            limit = GameDataManager.Instance.GetUpgradeLimitRarity(limit);
        }
        #endregion

        #region 운명 재설정
        internal void DestinyRecast()
        {
            var result = _agentChangeSystem.ChangeRandomUnit(this);

            if (result.HasValue)
            {
                agentTemplate = result.Value.agentTemplate;
                agentUnit = result.Value.agentUnit;
                result.Value.action?.Invoke(this);
            }
        }
        #endregion
    }
}