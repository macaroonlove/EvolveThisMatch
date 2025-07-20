using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class AgentReturnSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private FX _returnFX;

        private PoolSystem _poolSystem;
        private CrystalSystem _crystalSystem;

        public event UnityAction<AgentBattleData> onDeinitializedUnit;

        public void Initialize()
        {
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            _crystalSystem = BattleManager.Instance.GetSubSystem<CrystalSystem>();
        }

        public void Deinitialize()
        {

        }

        #region 유닛 반환
        public async void ReturnUnit(AgentBattleData agentData)
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

            onDeinitializedUnit?.Invoke(agentData);
        }
        #endregion

        #region 유닛 교체
        internal void ReturnUnit_Change(AgentBattleData agentData)
        {
            // 유닛의 능력 종료
            agentData.agentUnit.Deinitialize();

            // 표지판 반환
            if (agentData.signBoard != null)
            {
                ReturnSignBoard(agentData.signBoard.gameObject);
                agentData.DeregistSignBoard();
            }

            // 유닛 오브젝트 반환
            _poolSystem.DeSpawn(agentData.agentUnit.gameObject);

            onDeinitializedUnit?.Invoke(agentData);
        }
        #endregion

        #region 출정 표지판 반환
        public void ReturnSignBoard(GameObject obj)
        {
            // 유닛 오브젝트 반환
            _poolSystem.DeSpawn(obj);
        }
        #endregion
    }
}