using System;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class AgentBattleData
    {
        private AgentChangeSystem _agentChangeSystem;

        private int _antiCheatToken;
        private int _antiCheatFrame;

        public AgentTemplate agentTemplate { get; private set; }
        public AgentUnit agentUnit { get; private set; }
        public SignBoard signBoard { get; private set; }
        public int sync { get; private set; }
        public int skillUnlock { get; private set; }
        public AgentRarityTemplate limit { get; private set; }
        public TileController mountTile { get; private set; }

        public event UnityAction<AgentBattleData> onSyncIncrease;

        public AgentBattleData(AgentUnit agentUnit, AgentTemplate agentTemplate)
        {
            this.agentUnit = agentUnit;
            this.agentTemplate = agentTemplate;

            this.sync = 1;
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

        #region 동기화
        public void PrepareSyncIncrease(int token)
        {
            _antiCheatToken = token;
            _antiCheatFrame = Time.frameCount;
        }

        /// <returns>-1: 비정상적인 접근 | 1: 성공</returns>
        public int ApplySyncIncrease(int token)
        {
            if (token != _antiCheatToken) return -1;
            if (Time.frameCount != _antiCheatFrame) return -1;

            sync++;
            skillUnlock = sync / 5;
            onSyncIncrease?.Invoke(this);
            _antiCheatToken = 0;

            return 1;
        }
        #endregion

        #region 승격 제한 업그레이드
        public void PrepareUpgradeLimit(int token)
        {
            _antiCheatToken = token;
            _antiCheatFrame = Time.frameCount;
        }

        /// <returns>-1: 비정상적인 접근 | 0: 실패 | 1: 성공 | 2: 대성공 | 3: 승화 | 4: 초월</returns>
        public int ApplyUpgradeLimit(int token)
        {
            if (token != _antiCheatToken) return -1;
            if (Time.frameCount != _antiCheatFrame) return -1;

            var newLimit = limit.agentLimitData.GetUpgradeLimitResult();
            if (newLimit == null) return 0;

            int result = newLimit.rarity - limit.rarity;
            limit = newLimit;
            _antiCheatToken = 0;

            return result;
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