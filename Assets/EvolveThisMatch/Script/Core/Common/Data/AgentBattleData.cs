using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AgentBattleData
    {
        private AgentCreateSystem _agentCreateSystem;

        internal AgentTemplate agentTemplate { get; private set; }
        internal AgentUnit agentUnit { get; private set; }
        internal SignBoard signBoard { get; private set; }
        internal int level { get; private set; }
        internal AgentRarityTemplate limit { get; private set; }
        internal TileController mountTile { get; private set; }

        public AgentBattleData(AgentUnit agentUnit, AgentTemplate agentTemplate)
        {
            this.agentUnit = agentUnit;
            this.agentTemplate = agentTemplate;

            this.level = 1;
            this.limit = GameDataManager.Instance.GetAgentRandomRarity();

            _agentCreateSystem = BattleManager.Instance.GetSubSystem<AgentCreateSystem>();
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

        internal void DeregistSignBoard()
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
            agentUnit.Deinitialize();

            var result = _agentCreateSystem.ChangeRandomUnit(agentUnit, limit);

            agentTemplate = result.template;
            agentUnit = result.unit;
            agentUnit.transform.position = mountTile.transform.position;
            agentUnit.Initialize(this);

            // 스폰 이펙트
            result.spawnFX.Play(agentUnit);
        }
        #endregion
    }
}