using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class AgentCreateSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private GameObject _signBoardPrefab;
        [SerializeField] private FX _spawnFX;

        private AgentReturnSystem _agentReturnSystem;
        private PoolSystem _poolSystem;
        private TileSystem _tileSystem;
        private List<AgentTemplate> _ownedAgentTemplates = new List<AgentTemplate>();

        internal event UnityAction<AgentBattleData> onInitializedUnit;

        public void Initialize()
        {
            _agentReturnSystem = BattleManager.Instance.GetSubSystem<AgentReturnSystem>();
            _tileSystem = BattleManager.Instance.GetSubSystem<TileSystem>();
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            
            var ownedAgents = GameDataManager.Instance.profileSaveData.ownedAgents;
            foreach(var agent in ownedAgents)
            {
                _ownedAgentTemplates.Add(GameDataManager.Instance.GetAgentTemplateById(agent.id));
            }
        }

        public void Deinitialize()
        {

        }

        #region 유닛 소환
        /// <summary>
        /// 등급 제한 없이 랜덤 유닛
        /// </summary>
        internal bool CreateRandomUnit()
        {
            var rand = GameDataManager.Instance.GetAgentRandomRarity();
            var filtered = _ownedAgentTemplates.Where(t => t.rarity.rarity == rand.rarity).ToList();

            int index = Random.Range(0, filtered.Count);
            var template = filtered[index];

            return CreateUnit(template);
        }

        /// <summary>
        /// 최소 등급 제한 랜덤 유닛
        /// </summary>
        internal bool CreateRandomUnit(EAgentRarity rarity)
        {
            var rand = GameDataManager.Instance.GetAgentRandomRarity();
            var finalRarity = (rand.rarity < rarity) ? rand.rarity : rarity;
            var filtered = _ownedAgentTemplates.Where(t => t.rarity.rarity == finalRarity).ToList();

            int index = Random.Range(0, filtered.Count);
            var template = filtered[index];

            return CreateUnit(template);
        }

        private bool CreateUnit(AgentTemplate template)
        {
            // 타일 위치 가져오기
            var tile = _tileSystem.GetPlaceAbleTile(template.id);

            // TODO: 더 이상 소환할 수 없다고 팝업 띄워주기
            if (tile == null) return false;

            // 유닛 생성하기
            var obj = _poolSystem.Spawn(template.prefab, transform);

            if (obj.TryGetComponent(out AgentUnit unit))
            {
                var agentData = tile.PlaceUnit(unit, template);
                
                // 유닛 초기화
                unit.Initialize(agentData);

                // 스폰 이펙트
                _spawnFX.Play(unit);

                onInitializedUnit?.Invoke(agentData);
            }
            else
            {
                _poolSystem.DeSpawn(obj);
                return false;
            }

            return true;
        }
        #endregion

        #region 유닛 교체
        public struct ChangeUnitResult
        {
            public AgentUnit unit;
            public AgentTemplate template;
            public FX spawnFX;

            public ChangeUnitResult(AgentUnit unit, AgentTemplate template, FX spawnFX)
            {
                this.unit = unit;
                this.template = template;
                this.spawnFX = spawnFX;
            }
        }

        internal ChangeUnitResult ChangeRandomUnit(AgentBattleData agentData)
        {
            var filtered = _ownedAgentTemplates.Where(t => t.rarity.rarity <= agentData.limit.rarity && t != agentData.agentUnit.template).ToList();

            int index = Random.Range(0, filtered.Count);
            var template = filtered[index];

            var obj = _poolSystem.Spawn(template.prefab, transform);
            if (obj.TryGetComponent(out AgentUnit unit))
            {
                _agentReturnSystem.ReturnUnit_Change(agentData);
                _poolSystem.DeSpawn(agentData.agentUnit.gameObject);

                agentData.agentUnit.transform.position = agentData.mountTile.transform.position;
                unit.Initialize(agentData);

                onInitializedUnit?.Invoke(agentData);

                return new ChangeUnitResult(unit, template, _spawnFX);
            }

            _poolSystem.DeSpawn(obj);
            return new ChangeUnitResult(null, null, null);
        }
        #endregion

        #region 표지판 생성
        internal SignBoard CreateSignBoard(AgentBattleData selectedData)
        {
            // 위치
            var pos = selectedData.mountTile.transform.position;

            // 표지판 생성하기
            var obj = _poolSystem.Spawn(_signBoardPrefab, transform);

            if (obj.TryGetComponent(out SignBoard signBoard))
            {
                signBoard.transform.position = pos;

                signBoard.Initialize(selectedData.agentUnit);

                return signBoard;
            }
            else
            {
                _poolSystem.DeSpawn(obj);
                return null;
            }
        }
        #endregion
    }
}