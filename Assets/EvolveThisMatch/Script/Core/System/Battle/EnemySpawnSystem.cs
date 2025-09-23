using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class EnemySpawnSystem : MonoBehaviour, IBattleSystem
    {
        private EnemySystem _enemySystem;
        private PoolSystem _poolSystem;

        public void Initialize()
        {
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            _enemySystem = BattleManager.Instance.GetSubSystem<EnemySystem>();
        }

        public void Deinitialize()
        {
            _poolSystem = null;
        }

        public EnemyUnit SpawnUnit(EnemyData data, Vector3 pos, bool isIdle)
        {
            // 유닛 생성하기
            var obj = _poolSystem.Spawn(data.template.prefab, transform);

            // 유닛 위치 정해주기 (위치가 타일과 같을 경우 타일에서 위치 정해주기도 가능)
            obj.transform.SetPositionAndRotation(pos, Quaternion.identity);

            if (obj.TryGetComponent(out EnemyUnit unit))
            {
                // 유닛 초기화
                unit.Initialize(data, isIdle);

                // 유닛 등록
                _enemySystem.Regist(unit);

                return unit;
            }
            else
            {
                _poolSystem.DeSpawn(obj);
                return null;
            }
        }
    }
}