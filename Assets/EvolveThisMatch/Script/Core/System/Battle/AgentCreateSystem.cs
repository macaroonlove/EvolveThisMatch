using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AgentCreateSystem : MonoBehaviour, IBattleSystem
    {
        private AllySystem _allySystem;
        private PoolSystem _poolSystem;

        public void Initialize()
        {
            _allySystem = BattleManager.Instance.GetSubSystem<AllySystem>();
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
        }

        public void Deinitialize()
        {

        }

        internal bool CreateUnit(AgentTemplate template, Vector3 pos)
        {
            // 생성할 유닛 불러오기 (선택의 경우 매개변수로 아이디 등을 받음)
            //var templates = GameDataManager.Instance.ownedAgentTemplate;
            //int index = Random.Range(0, templates.Count);
            //var template = templates[index];

            // 생성할 위치 찾기 (선택의 경우 매개변수로 위치를 받음)
            //Vector3 pos = new Vector3(0, 0, 0);

            // 유닛 생성하기
            var obj = _poolSystem.Spawn(template.prefab, transform);

            // 유닛 위치 정해주기 (위치가 타일과 같을 경우 타일에서 위치 정해주기도 가능)
            obj.transform.SetPositionAndRotation(pos, Quaternion.identity);

            if (obj.TryGetComponent(out AgentUnit unit))
            {
                // 유닛 초기화
                unit.Initialize(template);

                // 유닛 등록
                _allySystem.Regist(unit);
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