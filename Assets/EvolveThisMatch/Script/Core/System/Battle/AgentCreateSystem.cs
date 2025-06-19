using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AgentCreateSystem : MonoBehaviour, IBattleSystem
    {
        private PoolSystem _poolSystem;
        private TileSystem _tileSystem;

        private List<Save.ProfileSaveData.Agent> _ownedAgents = new List<Save.ProfileSaveData.Agent>();

        public void Initialize()
        {
            _tileSystem = BattleManager.Instance.GetSubSystem<TileSystem>();
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            _ownedAgents = GameDataManager.Instance.profileSaveData.ownedAgents;
        }

        public void Deinitialize()
        {

        }

        internal bool CreateRandomUnit()
        {
            int index = Random.Range(0, _ownedAgents.Count);
            var template = GameDataManager.Instance.GetAgentTemplateById(index);

            return CreateUnit(template);
        }

        private bool CreateUnit(AgentTemplate template)
        {
            // 유닛 생성하기
            var obj = _poolSystem.Spawn(template.prefab, transform);

            // 타일 위치 가져오기
            var tile = _tileSystem.GetPlaceAbleTile(template.id);

            if (obj.TryGetComponent(out AgentUnit unit))
            {
                // 유닛 초기화
                unit.Initialize(template);

                tile.PlaceUnit(unit);
            }
            else
            {
                _poolSystem.DeSpawn(obj);
                return false;
            }

            return true;
        }
    }
}