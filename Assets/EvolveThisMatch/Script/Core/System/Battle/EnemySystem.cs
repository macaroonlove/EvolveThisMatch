using FrameWork;
using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 적 유닛을 관리하는 클래스
    /// (유틸리티 메서드)
    /// </summary>
    public class EnemySystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField, ReadOnly] private List<EnemyUnit> _enemies = new List<EnemyUnit>();

        private AttackRangeRenderer _attackRangeRenderer;

        public int enemyCount => _enemies.Count;

        public event UnityAction<Unit> onRegist;
        public event UnityAction<Unit> onDeregist;
        public event UnityAction<int> onDeath;

        public void Initialize()
        {
            _attackRangeRenderer = BattleManager.Instance.GetSubSystem<AttackRangeRenderer>();
        }

        public void Deinitialize()
        {
            // 유닛 오브젝트 모두 파괴
            foreach (var enemy in _enemies)
            {
                if (enemy == null) continue;

                var obj = enemy.gameObject;
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            _enemies.Clear();
        }

        internal void Regist(EnemyUnit enemy)
        {
            _enemies.Add(enemy);

            onRegist?.Invoke(enemy);
        }

        internal void Deregist(EnemyUnit enemy)
        {
            _enemies.Remove(enemy);

            onDeregist?.Invoke(enemy);
        }

        internal void DeathEnemy(EnemyUnit enemy)
        {
            onDeath?.Invoke(enemy.id);

            Deregist(enemy);
        }

        #region 유틸리티 메서드
        /// <summary>
        /// 등록된 모든 적 유닛을 반환
        /// </summary>
        public List<EnemyUnit> GetAllEnemies()
        {
            return _enemies;
        }

        #region 원 범위 안쪽의 적군 유닛을 반환
        /// <summary>
        /// 원 범위 안쪽의 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        public List<EnemyUnit> GetEnemiesInCircle(Vector2 unitPos, float radius, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllEnemiesInCircle(unitPos, radius);
            }
            else
            {
                return GetSortedEnemiesInCircle(unitPos, radius, maxCount);
            }
        }

        private List<EnemyUnit> GetAllEnemiesInCircle(Vector2 unitPos, float radius)
        {
            List<EnemyUnit> enemies = new List<EnemyUnit>();

            radius *= radius;

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    var distance = (enemy.cellPos - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        enemies.Add((enemy));
                    }
                }
            }

            return enemies;
        }

        private List<EnemyUnit> GetSortedEnemiesInCircle(Vector2 unitPos, float radius)
        {
            PriorityQueue<EnemyUnit> priorityQueue = new PriorityQueue<EnemyUnit>();

            radius *= radius;

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    var distance = (enemy.cellPos - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        priorityQueue.Enqueue(enemy, distance);
                    }
                }
            }

            List<EnemyUnit> enemies = new List<EnemyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                enemies.Add(priorityQueue.Dequeue());
            }

            return enemies;
        }

        private List<EnemyUnit> GetSortedEnemiesInCircle(Vector2 unitPos, float radius, int maxCount)
        {
            PriorityQueue<EnemyUnit> priorityQueue = new PriorityQueue<EnemyUnit>();

            radius *= radius;

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    var distance = (enemy.cellPos - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        priorityQueue.Enqueue(enemy, distance);

                        if (priorityQueue.Count > maxCount)
                        {
                            priorityQueue.Dequeue();
                        }
                    }
                }
            }

            List<EnemyUnit> enemies = new List<EnemyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                enemies.Add(priorityQueue.Dequeue());
            }

            return enemies;
        }
        #endregion

        #region 직선 범위 안쪽의 적군 유닛을 반환
        /// <summary>
        /// 직선 범위 안쪽의 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<EnemyUnit> GetEnemiesInStraight(Vector2 unitPos, Vector2 targetDir, float range, float width, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllEnemiesInStraight(unitPos, targetDir, range, width);
            }
            else
            {
                return GetSortedEnemiesInStraight(unitPos, targetDir, range, width, maxCount);
            }
        }

        private List<EnemyUnit> GetAllEnemiesInStraight(Vector2 unitPos, Vector2 targetDir, float range, float width)
        {
            List<EnemyUnit> enemies = new List<EnemyUnit>();

            targetDir = targetDir.normalized;
            float widthThreshold = (width * 0.5f) * (width * 0.5f);

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    Vector2 dirVector = enemy.cellPos - unitPos;

                    // 유닛을 기준으로한 정면 거리
                    float forwardDist = Vector2.Dot(targetDir, dirVector);
                    if (forwardDist < 0 || forwardDist > range) continue;

                    // 유닛을 기준으로한 측면 거리
                    float sideDist = Vector3.Cross(targetDir, dirVector).sqrMagnitude;
                    if (sideDist <= widthThreshold)
                    {
                        enemies.Add(enemy);
                    }
                }
            }

            return enemies;
        }

        private List<EnemyUnit> GetSortedEnemiesInStraight(Vector2 unitPos, Vector2 targetDir, float range, float width)
        {
            PriorityQueue<EnemyUnit> priorityQueue = new PriorityQueue<EnemyUnit>();

            targetDir = targetDir.normalized;
            float widthThreshold = (width * 0.5f) * (width * 0.5f);

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    Vector2 dirVector = enemy.cellPos - unitPos;

                    // 유닛을 기준으로한 정면 거리
                    float forwardDist = Vector2.Dot(targetDir, dirVector);
                    if (forwardDist < 0 || forwardDist > range) continue;

                    // 유닛을 기준으로한 측면 거리
                    float sideDist = Vector3.Cross(targetDir, dirVector).sqrMagnitude;
                    if (sideDist <= widthThreshold)
                    {
                        priorityQueue.Enqueue(enemy, forwardDist + sideDist);
                    }
                }
            }

            List<EnemyUnit> enemies = new List<EnemyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                enemies.Add(priorityQueue.Dequeue());
            }

            return enemies;
        }

        private List<EnemyUnit> GetSortedEnemiesInStraight(Vector2 unitPos, Vector2 targetDir, float range, float width, int maxCount)
        {
            PriorityQueue<EnemyUnit> priorityQueue = new PriorityQueue<EnemyUnit>();

            targetDir = targetDir.normalized;
            float widthThreshold = (width * 0.5f) * (width * 0.5f);

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    Vector2 dirVector = enemy.cellPos - unitPos;

                    // 유닛을 기준으로한 정면 거리
                    float forwardDist = Vector2.Dot(targetDir, dirVector);
                    if (forwardDist < 0 || forwardDist > range) continue;

                    // 유닛을 기준으로한 측면 거리
                    float sideDist = Vector3.Cross(targetDir, dirVector).sqrMagnitude;
                    if (sideDist <= widthThreshold)
                    {
                        priorityQueue.Enqueue(enemy, forwardDist + sideDist);

                        if (priorityQueue.Count > maxCount)
                        {
                            priorityQueue.Dequeue();
                        }
                    }
                }
            }

            List<EnemyUnit> enemies = new List<EnemyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                enemies.Add(priorityQueue.Dequeue());
            }

            return enemies;
        }
        #endregion

        #region 시야각 안쪽의 아군 유닛을 반환
        /// <summary>
        /// 시야각 안쪽의 적군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<EnemyUnit> GetEnemiesInCone(Vector2 unitPos, Vector2 targetDir, float range, int angle, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllEnemiesInCone(unitPos, targetDir, range, angle);
            }
            else
            {
                return GetSortedEnemiesInCone(unitPos, targetDir, range, angle, maxCount);
            }
        }

        private List<EnemyUnit> GetAllEnemiesInCone(Vector2 unitPos, Vector2 targetDir, float range, int angle)
        {
            List<EnemyUnit> enemies = new List<EnemyUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    Vector2 dirVector = enemy.cellPos - unitPos;
                    float distance = dirVector.sqrMagnitude;

                    if (distance <= range)
                    {
                        float dot = Vector2.Dot(targetDir, dirVector);

                        if (dot * dot >= cos * distance)
                        {
                            enemies.Add(enemy);
                        }
                    }
                }
            }

            return enemies;
        }

        private List<EnemyUnit> GetSortedEnemiesInCone(Vector2 unitPos, Vector2 targetDir, float range, int angle)
        {
            PriorityQueue<EnemyUnit> priorityQueue = new PriorityQueue<EnemyUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    Vector2 dirVector = enemy.cellPos - unitPos;
                    float distance = dirVector.sqrMagnitude;

                    if (distance <= range)
                    {
                        float dot = Vector2.Dot(targetDir, dirVector);

                        if (dot * dot >= cos * distance)
                        {
                            priorityQueue.Enqueue(enemy, distance);
                        }
                    }
                }
            }

            List<EnemyUnit> enemies = new List<EnemyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                enemies.Add(priorityQueue.Dequeue());
            }

            return enemies;
        }

        private List<EnemyUnit> GetSortedEnemiesInCone(Vector2 unitPos, Vector2 targetDir, float range, float angle, int maxCount)
        {
            PriorityQueue<EnemyUnit> priorityQueue = new PriorityQueue<EnemyUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    Vector2 dirVector = enemy.cellPos - unitPos;
                    float distance = dirVector.sqrMagnitude;

                    if (distance <= range)
                    {
                        float dot = Vector2.Dot(targetDir, dirVector);

                        if (dot * dot >= cos * distance)
                        {
                            priorityQueue.Enqueue(enemy, distance);

                            if (priorityQueue.Count > maxCount)
                            {
                                priorityQueue.Dequeue();
                            }
                        }
                    }
                }
            }

            List<EnemyUnit> enemies = new List<EnemyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                enemies.Add(priorityQueue.Dequeue());
            }

            return enemies;
        }
        #endregion

        #region 라인 범위 안쪽의 적군 유닛을 반환
        /// <summary>
        /// 원 범위 안쪽의 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<EnemyUnit> GetEnemiesInLine(Vector2 unitPos, float range, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllEnemiesInLine(range);
            }
            else
            {
                return GetSortedEnemiesInLine(unitPos, range, maxCount);
            }
        }

        private List<EnemyUnit> GetAllEnemiesInLine(float range)
        {
            List<EnemyUnit> enemies = new List<EnemyUnit>();

            float tileRadius = 1.25f;
            float sqrRange = tileRadius * tileRadius;

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy == null || !enemy.isActiveAndEnabled) continue;

                Vector2 enemyPos = enemy.transform.position;

                for (int i = 0; i < range; i++)
                {
                    foreach (var point in _attackRangeRenderer.lines[i])
                    {
                        if ((enemyPos - point).sqrMagnitude <= sqrRange)
                        {
                            enemies.Add(enemy);
                            goto NextEnemy;
                        }
                    }
                }

                NextEnemy:;
            }

            return enemies;
        }

        private List<EnemyUnit> GetSortedEnemiesInLine(Vector2 unitPos, float range)
        {
            PriorityQueue<EnemyUnit> priorityQueue = new PriorityQueue<EnemyUnit>();

            float tileRadius = 1.25f;
            float sqrRange = tileRadius * tileRadius;

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy == null || !enemy.isActiveAndEnabled) continue;

                Vector2 enemyPos = enemy.transform.position;

                for (int i = 0; i < range; i++)
                {
                    foreach (var point in _attackRangeRenderer.lines[i])
                    {
                        if ((enemyPos - point).sqrMagnitude <= sqrRange)
                        {
                            var distance = (enemyPos - unitPos).sqrMagnitude;

                            priorityQueue.Enqueue(enemy, distance);
                            goto NextEnemy;
                        }
                    }
                }

                NextEnemy:;
            }

            List<EnemyUnit> enemies = new List<EnemyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                enemies.Add(priorityQueue.Dequeue());
            }

            return enemies;
        }

        private List<EnemyUnit> GetSortedEnemiesInLine(Vector2 unitPos, float range, int maxCount)
        {
            PriorityQueue<EnemyUnit> priorityQueue = new PriorityQueue<EnemyUnit>();

            float tileRadius = 1.25f;
            float sqrRange = tileRadius * tileRadius;

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy == null || !enemy.isActiveAndEnabled) continue;

                Vector2 enemyPos = enemy.transform.position;

                for (int i = 0; i < range; i++)
                {
                    foreach (var point in _attackRangeRenderer.lines[i])
                    {
                        if ((enemyPos - point).sqrMagnitude <= sqrRange)
                        {
                            var distance = (enemyPos - unitPos).sqrMagnitude;

                            priorityQueue.Enqueue(enemy, distance);

                            if (priorityQueue.Count > maxCount)
                            {
                                priorityQueue.Dequeue();
                            }

                            goto NextEnemy;
                        }
                    }
                }

                NextEnemy:;
            }

            List<EnemyUnit> enemies = new List<EnemyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                enemies.Add(priorityQueue.Dequeue());
            }

            return enemies;
        }
        #endregion

        #region 범위 내 공격 가능한 적군 유닛을 반환
        internal List<EnemyUnit> GetAttackableEnemies(Vector2 unitPos, float radius, EAttackType attackType, int maxCount = int.MaxValue)
        {
            var enemies = GetSortedEnemiesInCircle(unitPos, radius);

            return CheckAttackable(enemies, attackType, maxCount);
        }

        internal List<EnemyUnit> GetAttackableEnemies(Vector2 unitPos, Vector2 targetDir, float range, float width, EAttackType attackType, int maxCount = int.MaxValue)
        {
            var enemies = GetSortedEnemiesInStraight(unitPos, targetDir, range, width);

            return CheckAttackable(enemies, attackType, maxCount);
        }

        internal List<EnemyUnit> GetAttackableEnemies(Vector2 unitPos, Vector2 targetDir, float range, int angle, EAttackType attackType, int maxCount = int.MaxValue)
        {
            var enemies = GetSortedEnemiesInCone(unitPos, targetDir, range, angle);

            return CheckAttackable(enemies, attackType, maxCount);
        }

        internal List<EnemyUnit> GetAttackableEnemies(Vector2 unitPos, int range, EAttackType attackType, int maxCount = int.MaxValue)
        {
            var enemies = GetSortedEnemiesInLine(unitPos, range);

            return CheckAttackable(enemies, attackType, maxCount);
        }

        internal List<EnemyUnit> GetAllAttackableEnemies(EAttackType attackType)
        {
            var enemies = new List<EnemyUnit>();

            foreach (var enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    // 적이 공중 유닛일 떄, 원거리가 아니라면 공격 불가 (타워 디펜스라면 언덕 유닛일 때, 로 변경)
                    if (enemy.enemyData.template.MoveType == EMoveType.Sky && attackType != EAttackType.Far) continue;

                    // 공격 대상이 아니라면 타겟에 추가하지 않음
                    if (enemy.GetAbility<HitAbility>().finalTargetOfAttack == false) continue;

                    enemies.Add(enemy);
                }
            }

            return enemies;
        }

        private List<EnemyUnit> CheckAttackable(List<EnemyUnit> enemies, EAttackType attackType, int maxCount)
        {
            var healableEnemies = new List<EnemyUnit>(maxCount);

            foreach (var enemy in enemies)
            {
                // maxCount만큼 유닛을 찾았다면
                if (healableEnemies.Count >= maxCount) break;

                // 적이 공중 유닛일 떄, 원거리가 아니라면 공격 불가 (타워 디펜스라면 언덕 유닛일 때, 로 변경)
                if (enemy.enemyData.template.MoveType == EMoveType.Sky && attackType != EAttackType.Far) continue;

                // 공격 대상이 아니라면 타겟에 추가하지 않음
                if (enemy.GetAbility<HitAbility>().finalTargetOfAttack == false) continue;

                healableEnemies.Add(enemy);
            }

            return healableEnemies;
        }
        #endregion

        #region 범위 내 회복 가능한 적군 유닛을 반환
        internal List<EnemyUnit> GetHealableEnemies(Vector2 unitPos, float radius, int maxCount = int.MaxValue)
        {
            var enemies = GetSortedEnemiesInCircle(unitPos, radius);

            return CheckHealable(enemies, maxCount);
        }

        internal List<EnemyUnit> GetHealableEnemies(Vector2 unitPos, Vector2 targetDir, float range, float width, int maxCount = int.MaxValue)
        {
            var enemies = GetSortedEnemiesInStraight(unitPos, targetDir, range, width);

            return CheckHealable(enemies, maxCount);
        }

        internal List<EnemyUnit> GetHealableEnemies(Vector2 unitPos, Vector2 targetDir, float range, int angle, int maxCount = int.MaxValue)
        {
            var enemies = GetSortedEnemiesInCone(unitPos, targetDir, range, angle);

            return CheckHealable(enemies, maxCount);
        }

        internal List<EnemyUnit> GetAllHealableEnemies()
        {
            var enemies = new List<EnemyUnit>();

            foreach (var enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    // 회복 가능 유닛이 아니라면 타겟에 추가하지 않음
                    if (enemy.GetAbility<HealthAbility>().finalIsHealAble == false) continue;

                    enemies.Add(enemy);
                }
            }

            return enemies;
        }

        private List<EnemyUnit> CheckHealable(List<EnemyUnit> enemies, int maxCount)
        {
            var attackableEnemies = new List<EnemyUnit>(maxCount);

            foreach (var enemy in enemies)
            {
                // maxCount만큼 유닛을 찾았다면
                if (attackableEnemies.Count >= maxCount) break;

                // 회복 가능 유닛이 아니라면 타겟에 추가하지 않음
                if (enemy.GetAbility<HealthAbility>().finalIsHealAble == false) continue;

                attackableEnemies.Add(enemy);
            }

            return attackableEnemies;
        }
        #endregion

        /// <summary>
        /// 범위 내에 가장 가까운 적 유닛을 반환
        /// </summary>
        internal EnemyUnit GetNearestEnemy(Vector2 unitPos, float radius)
        {
            EnemyUnit enemyUnit = null;
            radius *= radius;
            float nearestDistance = Mathf.Infinity;

            foreach (EnemyUnit enemy in _enemies)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                {
                    float distance = (enemy.cellPos - unitPos).sqrMagnitude;

                    if (distance < nearestDistance && distance <= radius)
                    {
                        enemyUnit = enemy;
                        nearestDistance = distance;
                    }
                }
            }

            return enemyUnit;
        }
        #endregion
    }
}