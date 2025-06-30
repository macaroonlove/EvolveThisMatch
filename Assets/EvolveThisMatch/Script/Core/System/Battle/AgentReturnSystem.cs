using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AgentReturnSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private FX _returnFX;

        private PoolSystem _poolSystem;
        private CrystalSystem _crystalSystem;
        private List<AgentTemplate> _ownedAgentTemplates = new List<AgentTemplate>();

        public void Initialize()
        {
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            _crystalSystem = BattleManager.Instance.GetSubSystem<CrystalSystem>();
            
            var ownedAgents = GameDataManager.Instance.profileSaveData.ownedAgents;
            foreach(var agent in ownedAgents)
            {
                _ownedAgentTemplates.Add(GameDataManager.Instance.GetAgentTemplateById(agent.id));
            }
        }

        public void Deinitialize()
        {

        }

        #region 유닛 반환
        internal async void ReturnUnit(AgentBattleData agentData)
        {
            // 유닛 반환 FX
            _returnFX.Play(agentData.agentUnit);

            await UniTask.Delay(600);

            // 유닛의 능력 종료
            agentData.agentUnit.Deinitialize();

            // 유닛 오브젝트 반환
            _poolSystem.DeSpawn(agentData.agentUnit.gameObject);

            // 유닛의 등급별로 반환받을 크리스탈 설정
            int returnCrystal = 0;
            switch (agentData.agentTemplate.rarity.rarity)
            {
                case EAgentRarity.Myth:
                    returnCrystal = 5;
                    break;
                case EAgentRarity.Legend:
                    returnCrystal = 3;
                    break;
                case EAgentRarity.Epic:
                    returnCrystal = 1;
                    break;
            }

            if (returnCrystal > 0)
            {
                _crystalSystem.AddCrystal(returnCrystal);
            }

            // 유닛을 타일에서 반환
            agentData.mountTile.ReturnUnit();
        }
        #endregion
    }
}