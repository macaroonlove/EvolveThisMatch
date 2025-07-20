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

        private PoolSystem _poolSystem;
        private TileSystem _tileSystem;
        private List<AgentTemplate> _ownedAgentTemplates = new List<AgentTemplate>();

        public event UnityAction<AgentBattleData> onInitializedUnit;

        public void Initialize()
        {
            _tileSystem = BattleManager.Instance.GetSubSystem<TileSystem>();
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();

            var ownedAgents = GameDataManager.Instance.profileSaveData.ownedAgents;
            foreach (var agent in ownedAgents)
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
        public bool CreateRandomUnit()
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
        public bool CreateRandomUnit(EAgentRarity rarity)
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
        internal ChangeAgentData? CreateUnit_Change(AgentBattleData agentData)
        {
            var filtered = _ownedAgentTemplates.Where(t => t.rarity.rarity <= agentData.limit.rarity && t != agentData.agentUnit.template).ToList();

            int index = Random.Range(0, filtered.Count);
            var template = filtered[index];

            var obj = _poolSystem.Spawn(template.prefab, transform);

            if (obj.TryGetComponent(out AgentUnit unit))
            {
                return new ChangeAgentData(unit, template, OnChangedComplete);
            }

            _poolSystem.DeSpawn(obj);
            return null;
        }

        private void OnChangedComplete(AgentBattleData agentData)
        {
            var agentUnit = agentData.agentUnit;
            agentUnit.transform.position = agentData.mountTile.transform.position;
            agentUnit.Initialize(agentData);

            // 스폰 이펙트
            _spawnFX.Play(agentUnit);

            onInitializedUnit?.Invoke(agentData);
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