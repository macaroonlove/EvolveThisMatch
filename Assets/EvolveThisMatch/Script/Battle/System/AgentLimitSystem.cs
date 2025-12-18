using EvolveThisMatch.Core;
using UnityEngine;

namespace EvolveThisMatch.Battle
{
    public class AgentLimitSystem : MonoBehaviour, IBattleSystem
    {
        private CrystalSystem _crystalSystem;

        public void Initialize()
        {
            _crystalSystem = BattleManager.Instance.GetSubSystem<CrystalSystem>();
        }

        public void Deinitialize()
        {
            _crystalSystem = null;
        }

        /// <summary>
        /// 승격 제한 업그레이드 요청
        /// </summary>
        public int RequestUpgradeLimit(AgentBattleData data)
        {
            if (data.limit.rarity == EAgentRarity.Myth) return -2;

            int needCrystal = 1;

            if (!_crystalSystem.PayCrystal(needCrystal)) return -3;

            // 외부에서 메서드에 직접 접근할 경우를 대비하여 단발성 토큰 부여
            int token = Random.Range(int.MinValue, int.MaxValue);
            
            data.PrepareUpgradeLimit(token);
            return data.ApplyUpgradeLimit(token);
        }
    }
}