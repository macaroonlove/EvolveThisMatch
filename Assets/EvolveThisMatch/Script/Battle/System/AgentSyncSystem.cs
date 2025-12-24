using EvolveThisMatch.Core;
using UnityEngine;

namespace EvolveThisMatch.Battle
{
    public class AgentSyncSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private GlobalStatusTemplate _syncTemplate;

        private CoinSystem _coinSystem;
        private GlobalStatusSystem _globalStatusSystem;

        public void Initialize()
        {
            _coinSystem = BattleManager.Instance.GetSubSystem<CoinSystem>();
            _globalStatusSystem = CoreManager.Instance.GetSubSystem<GlobalStatusSystem>();

            _globalStatusSystem.ApplyGlobalStatus(_syncTemplate, int.MaxValue, null);
        }

        public void Deinitialize()
        {
            _coinSystem = null;
            _globalStatusSystem = null;
        }

        /// <summary>
        /// 동기화율 상승 요청
        /// </summary>
        public int RequestIncreaseSync(AgentBattleData data)
        {
            int needCoin = GetNeedCoin(data);

            if (needCoin <= 0 || data.sync > 15) return -2;

            if (!_coinSystem.PayCoin(needCoin)) return -3;

            // 외부에서 메서드에 직접 접근할 경우를 대비하여 단발성 토큰 부여
            int token = Random.Range(int.MinValue, int.MaxValue);
            
            data.PrepareSyncIncrease(token);
            return data.ApplySyncIncrease(token);
        }

        /// <summary>
        /// 동기화 상승에 필요한 코인의 수를 반환
        /// </summary>
        public int GetNeedCoin(AgentBattleData data)
        {
            var syncData = data.agentUnit.template.rarity.agentSyncData;
            return syncData.GetNeedCoin(data.sync);
        }
    }
}