using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class EntitySpawnAbility : AlwaysAbility
    {
        private PoolSystem _poolSystem;

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
        }

        internal override void Deinitialize()
        {
            _poolSystem = null;
        }

        internal void SpawnProjectile(GameObject prefab, ESpawnPoint spawnPoint, Unit targetUnit, UnityAction<Unit, Unit> action)
        {
            var spawnPosition = GetSpawnPoint(spawnPoint);

            var projectile = _poolSystem.Spawn(prefab).GetComponent<UnitProjectile>();
            projectile.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
            projectile.Initialize(unit, targetUnit, action);
        }

        internal void SpawnProjectile(GameObject prefab, ESpawnPoint spawnPoint, Vector3 targetVector, UnityAction<Unit, Unit> action)
        {
            var spawnPosition = GetSpawnPoint(spawnPoint);

            var projectile = _poolSystem.Spawn(prefab).GetComponent<PointProjectile>();
            projectile.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
            projectile.Initialize(unit, targetVector, action);
        }
        
        internal void SpawnTrap(GameObject prefab, Vector3 targetVector, UnityAction<Unit, Unit> action)
        {
            var trap = _poolSystem.Spawn(prefab).GetComponent<Trap>();
            trap.transform.SetPositionAndRotation(targetVector, Quaternion.identity);
            trap.Initialize(unit, action);
        }

        internal void SpawnTrap(GameObject prefab, Vector3 targetVector, float duration, UnityAction<Unit, Unit> action)
        {
            var trap = _poolSystem.Spawn(prefab, duration).GetComponent<Trap>();
            trap.transform.SetPositionAndRotation(targetVector, Quaternion.identity);
            trap.Initialize(unit, action);
        }

        #region 스폰 위치 불러오기
        private Vector3 GetSpawnPoint(ESpawnPoint spawnPoint)
        {
            Vector3 point = unit.transform.position;

            switch (spawnPoint)
            {
                case ESpawnPoint.Head:
                    if (unit.headPoint != null)
                    {
                        point = unit.headPoint.position;
                    }
                    break;
                case ESpawnPoint.Body:
                    if (unit.bodyPoint != null)
                    {
                        point = unit.bodyPoint.position;
                    }
                    break;
                case ESpawnPoint.LeftHand:
                    if (unit.leftHandPoint != null)
                    {
                        point = unit.leftHandPoint.position;
                    }
                    break;
                case ESpawnPoint.RightHand:
                    if (unit.rightHandPoint != null)
                    {
                        point = unit.rightHandPoint.position;
                    }
                    break;
                case ESpawnPoint.Foot:
                    if (unit.footPoint != null)
                    {
                        point = unit.footPoint.position;
                    }
                    break;
                case ESpawnPoint.ProjectileHit:
                    if (unit.projectileHitPoint != null)
                    {
                        point = unit.projectileHitPoint.position;
                    }
                    break;
            }

            return point;
        }
        #endregion
    }
}