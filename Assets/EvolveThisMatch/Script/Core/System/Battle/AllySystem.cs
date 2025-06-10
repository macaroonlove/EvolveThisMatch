using FrameWork;
using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 아군 유닛을 관리하는 클래스
    /// (유틸리티 메서드)
    /// </summary>
    public class AllySystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField, ReadOnly] private List<AgentUnit> _agents = new List<AgentUnit>();
        [SerializeField, ReadOnly] private List<SummonUnit> _summons = new List<SummonUnit>();

        internal event UnityAction<Unit> onRegist;

        public void Initialize()
        {

        }

        public void Deinitialize()
        {
            // 유닛 오브젝트 모두 파괴
            foreach (var agent in _agents)
            {
                Destroy(agent.gameObject);
            }

            foreach (var summon in _summons)
            {
                Destroy(summon.gameObject);
            }
        }

        #region 아군 유닛 등록·해제
        internal void Regist(AgentUnit agent)
        {
            _agents.Add(agent);

            onRegist?.Invoke(agent);
        }

        internal void Deregist(AgentUnit agent)
        {
            _agents.Remove(agent);
        }
        #endregion

        #region 소환수 등록·해제
        internal void Regist(SummonUnit summon)
        {
            _summons.Add(summon);

            onRegist?.Invoke(summon);
        }

        internal void Deregist(SummonUnit summon)
        {
            _summons.Remove(summon);
        }
        #endregion

        #region 유틸리티 메서드
        /// <summary>
        /// 등록된 모든 아군 유닛을 반환
        /// </summary>
        internal List<AllyUnit> GetAllAllies(EUnitType unitType)
        {
            List<AllyUnit> allies = new List<AllyUnit>();
            
            if ((unitType & EUnitType.Agent) != 0)
            {
                allies.AddRange(_agents);
            }

            if ((unitType & EUnitType.Summon) != 0)
            {
                allies.AddRange(_summons);
            }

            return allies;
        }

        #region 원 범위 안쪽의 아군 유닛을 반환
        /// <summary>
        /// 원 범위 안쪽의 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<AllyUnit> GetAlliesInCircle(Vector3 unitPos, float radius, EUnitType unitType, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllAlliesInCircle(unitPos, radius, unitType);
            }
            else
            {
                return GetSortedAlliesInCircle(unitPos, radius, unitType, maxCount);
            }
        }

        private List<AllyUnit> GetAllAlliesInCircle(Vector3 unitPos, float radius, EUnitType unitType)
        {
            List<AllyUnit> allies = new List<AllyUnit>();

            radius *= radius;

            void GetAllAlliesInCircle(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        var distance = (unit.transform.position - unitPos).sqrMagnitude;

                        if (distance <= radius)
                        {
                            allies.Add((unit));
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetAllAlliesInCircle(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetAllAlliesInCircle(_summons);

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInCircle(Vector3 unitPos, float radius, EUnitType unitType)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            radius *= radius;

            void GetSortedAlliesInCircle(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        var distance = (unit.transform.position - unitPos).sqrMagnitude;

                        if (distance <= radius)
                        {
                            priorityQueue.Enqueue(unit, distance);
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetSortedAlliesInCircle(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetSortedAlliesInCircle(_summons);

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInCircle(Vector3 unitPos, float radius, EUnitType unitType, int maxCount)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            radius *= radius;

            void GetSortedAlliesInCircle(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        var distance = (unit.transform.position - unitPos).sqrMagnitude;

                        if (distance <= radius)
                        {
                            priorityQueue.Enqueue(unit, distance);

                            if (priorityQueue.Count > maxCount)
                            {
                                priorityQueue.Dequeue();
                            }
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetSortedAlliesInCircle(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetSortedAlliesInCircle(_summons);

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }
        #endregion

        #region 직선 범위 안쪽의 아군 유닛을 반환
        /// <summary>
        /// 직선 범위 안쪽의 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<AllyUnit> GetAlliesInStraight(Vector3 unitPos, Vector3 targetDir, float range, float width, EUnitType unitType, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllAlliesInStraight(unitPos, targetDir, range, width, unitType);
            }
            else
            {
                return GetSortedAlliesInStraight(unitPos, targetDir, range, width, unitType, maxCount);
            }
        }

        private List<AllyUnit> GetAllAlliesInStraight(Vector3 unitPos, Vector3 targetDir, float range, float width, EUnitType unitType)
        {
            List<AllyUnit> allies = new List<AllyUnit>();

            targetDir = targetDir.normalized;
            float widthThreshold = (width * 0.5f) * (width * 0.5f);

            void GetAllAlliesInStraight(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        Vector3 dirVector = unit.transform.position - unitPos;

                        // 유닛을 기준으로한 정면 거리
                        float forwardDist = Vector3.Dot(targetDir, dirVector);
                        if (forwardDist < 0 || forwardDist > range) continue;

                        // 유닛을 기준으로한 측면 거리
                        float sideDist = Vector3.Cross(targetDir, dirVector).sqrMagnitude;
                        if (sideDist <= widthThreshold)
                        {
                            allies.Add(unit);
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetAllAlliesInStraight(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetAllAlliesInStraight(_summons);

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInStraight(Vector3 unitPos, Vector3 targetDir, float range, float width, EUnitType unitType)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            targetDir = targetDir.normalized;
            float widthThreshold = (width * 0.5f) * (width * 0.5f);

            void GetSortedAlliesInStraight(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        Vector3 dirVector = unit.transform.position - unitPos;

                        // 유닛을 기준으로한 정면 거리
                        float forwardDist = Vector3.Dot(targetDir, dirVector);
                        if (forwardDist < 0 || forwardDist > range) continue;

                        // 유닛을 기준으로한 측면 거리
                        float sideDist = Vector3.Cross(targetDir, dirVector).sqrMagnitude;
                        if (sideDist <= widthThreshold)
                        {
                            priorityQueue.Enqueue(unit, forwardDist + sideDist);
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetSortedAlliesInStraight(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetSortedAlliesInStraight(_summons);

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInStraight(Vector3 unitPos, Vector3 targetDir, float range, float width, EUnitType unitType, int maxCount)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            range *= range;
            float widthThreshold = ((width * width) / 4f) * targetDir.sqrMagnitude;

            void GetSortedAlliesInStraight(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        Vector3 dirVector = unit.transform.position - unitPos;

                        // 유닛을 기준으로한 정면 거리
                        float forwardDist = Vector3.Dot(targetDir, dirVector);
                        if (forwardDist < 0 || forwardDist > range) continue;

                        // 유닛을 기준으로한 측면 거리
                        float sideDist = Vector3.Cross(targetDir, dirVector).sqrMagnitude;
                        if (sideDist <= widthThreshold)
                        {
                            priorityQueue.Enqueue(unit, forwardDist + sideDist);

                            if (priorityQueue.Count > maxCount)
                            {
                                priorityQueue.Dequeue();
                            }
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetSortedAlliesInStraight(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetSortedAlliesInStraight(_summons);

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }
        #endregion

        #region 시야각 안쪽의 아군 유닛을 반환
        /// <summary>
        /// 시야각 안쪽의 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<AllyUnit> GetAlliesInCone(Vector3 unitPos, Vector3 targetDir, float range, int angle, EUnitType unitType, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllAlliesInCone(unitPos, targetDir, range, angle, unitType);
            }
            else
            {
                return GetSortedAlliesInCone(unitPos, targetDir, range, angle, unitType, maxCount);
            }
        }

        private List<AllyUnit> GetAllAlliesInCone(Vector3 unitPos, Vector3 targetDir, float range, int angle, EUnitType unitType)
        {
            List<AllyUnit> allies = new List<AllyUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            void GetAllAlliesInCone(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        Vector3 dirVector = unit.transform.position - unitPos;
                        float distance = dirVector.sqrMagnitude;

                        if (distance <= range)
                        {
                            float dot = Vector3.Dot(targetDir, dirVector);

                            if (dot * dot >= cos * distance)
                            {
                                allies.Add(unit);
                            }
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetAllAlliesInCone(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetAllAlliesInCone(_summons);

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInCone(Vector3 unitPos, Vector3 targetDir, float range, int angle, EUnitType unitType)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            void GetSortedAlliesInCone(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        Vector3 dirVector = unit.transform.position - unitPos;
                        float distance = dirVector.sqrMagnitude;

                        if (distance <= range)
                        {
                            float dot = Vector3.Dot(targetDir, dirVector);

                            if (dot * dot >= cos * distance)
                            {
                                priorityQueue.Enqueue(unit, distance);
                            }
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetSortedAlliesInCone(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetSortedAlliesInCone(_summons);

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInCone(Vector3 unitPos, Vector3 targetDir, float range, float angle, EUnitType unitType, int maxCount)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            void GetSortedAlliesInCone(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        Vector3 dirVector = unit.transform.position - unitPos;
                        float distance = dirVector.sqrMagnitude;

                        if (distance <= range)
                        {
                            float dot = Vector3.Dot(targetDir, dirVector);

                            if (dot * dot >= cos * distance)
                            {
                                priorityQueue.Enqueue(unit, distance);

                                if (priorityQueue.Count > maxCount)
                                {
                                    priorityQueue.Dequeue();
                                }
                            }
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetSortedAlliesInCone(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetSortedAlliesInCone(_summons);

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }
        #endregion

        #region 격자 범위 안쪽의 아군 유닛을 반환(2D 전용, X, Z 좌표)
        /// <summary>
        /// 격자 범위 안쪽의 아군 유닛을 반환 (unitPos와 가까운 유닛부터 반환)
        /// </summary>
        internal List<AllyUnit> GetAlliesInGrid(Vector2Int unitCellPos, List<Vector2Int> grid, EUnitType unitType, int maxCount = int.MaxValue)
        {
            if (maxCount == int.MaxValue)
            {
                return GetAllAlliesInGrid(unitCellPos, grid, unitType);
            }
            else
            {
                return GetSortedAlliesInGrid(unitCellPos, grid, unitType, maxCount);
            }
        }

        private List<AllyUnit> GetAllAlliesInGrid(Vector2Int unitCellPos, List<Vector2Int> grid, EUnitType unitType)
        {
            List<AllyUnit> allies = new List<AllyUnit>();

            void GetAllAlliesInGrid(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        var pos = unit.cellPos - unitCellPos;

                        if (grid.Contains(pos))
                        {
                            allies.Add(unit);
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetAllAlliesInGrid(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetAllAlliesInGrid(_summons);

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInGrid(Vector2Int unitCellPos, List<Vector2Int> grid, EUnitType unitType)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            void GetSortedAlliesInGrid(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        var pos = unit.cellPos - unitCellPos;

                        if (grid.Contains(pos))
                        {
                            var distance = (unit.cellPos - unitCellPos).sqrMagnitude;

                            priorityQueue.Enqueue(unit, distance);
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetSortedAlliesInGrid(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetSortedAlliesInGrid(_summons);

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInGrid(Vector2Int unitCellPos, List<Vector2Int> grid, EUnitType unitType, int maxCount)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            void GetSortedAlliesInGrid(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        var pos = unit.cellPos - unitCellPos;

                        if (grid.Contains(pos))
                        {
                            var distance = (unit.cellPos - unitCellPos).sqrMagnitude;

                            priorityQueue.Enqueue(unit, distance);

                            if (priorityQueue.Count > maxCount)
                            {
                                priorityQueue.Dequeue();
                            }
                        }
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) GetSortedAlliesInGrid(_agents);
            if ((unitType & EUnitType.Summon) != 0) GetSortedAlliesInGrid(_summons);

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }
        #endregion

        #region 범위 내 공격 가능한 아군 유닛을 반환
        internal List<AllyUnit> GetAttackableAllies(Vector3 unitPos, float radius, EAttackType attackType, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInCircle(unitPos, radius, unitType);

            return CheckAttackable(allies, attackType, maxCount);
        }

        internal List<AllyUnit> GetAttackableAllies(Vector3 unitPos, Vector3 targetDir, float range, float width, EAttackType attackType, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInStraight(unitPos, targetDir, range, width, unitType);

            return CheckAttackable(allies, attackType, maxCount);
        }

        internal List<AllyUnit> GetAttackableAllies(Vector3 unitPos, Vector3 targetDir, float range, int angle, EAttackType attackType, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInCone(unitPos, targetDir, range, angle, unitType);

            return CheckAttackable(allies, attackType, maxCount);
        }

        internal List<AllyUnit> GetAttackableAllies(Vector2Int unitCellPos, List<Vector2Int> grid, EAttackType attackType, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInGrid(unitCellPos, grid, unitType);

            return CheckAttackable(allies, attackType, maxCount);
        }

        internal List<AllyUnit> GetAllAttackableAllies(EAttackType attackType, EUnitType unitType)
        {
            var allies = new List<AllyUnit>();

            void CheckAttackable(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        // 적이 공중 유닛일 떄, 원거리가 아니라면 공격 불가 (타워 디펜스라면 언덕 유닛일 때, 로 변경)
                        if (unit.moveType == EMoveType.Sky && attackType != EAttackType.Far) continue;

                        // 공격 대상이 아니라면 타겟에 추가하지 않음
                        if (unit.GetAbility<HitAbility>().finalTargetOfAttack == false) continue;

                        allies.Add(unit);
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) CheckAttackable(_agents);
            if ((unitType & EUnitType.Summon) != 0) CheckAttackable(_summons);

            return allies;
        }

        private List<AllyUnit> CheckAttackable(List<AllyUnit> allies, EAttackType attackType, int maxCount)
        {
            var attackableAllies = new List<AllyUnit>(maxCount);

            foreach (var ally in allies)
            {
                // maxCount만큼 유닛을 찾았다면
                if (attackableAllies.Count >= maxCount) break;

                // 적이 공중 유닛일 떄, 원거리가 아니라면 공격 불가 (타워 디펜스라면 언덕 유닛일 때, 로 변경)
                if (ally.moveType == EMoveType.Sky && attackType != EAttackType.Far) continue;

                // 공격 대상이 아니라면 타겟에 추가하지 않음
                if (ally.GetAbility<HitAbility>().finalTargetOfAttack == false) continue;

                attackableAllies.Add(ally);
            }

            return attackableAllies;
        }
        #endregion

        #region 범위 내 회복 가능한 아군 유닛을 반환
        internal List<AllyUnit> GetHealableAllies(Vector3 unitPos, float radius, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInCircle(unitPos, radius, unitType);

            return CheckHealable(allies, maxCount);
        }

        internal List<AllyUnit> GetHealableAllies(Vector3 unitPos, Vector3 targetDir, float range, float width, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInStraight(unitPos, targetDir, range, width, unitType);

            return CheckHealable(allies, maxCount);
        }

        internal List<AllyUnit> GetHealableAllies(Vector3 unitPos, Vector3 targetDir, float range, int angle, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInCone(unitPos, targetDir, range, angle, unitType);

            return CheckHealable(allies, maxCount);
        }

        internal List<AllyUnit> GetHealableAllies(Vector2Int unitCellPos, List<Vector2Int> grid, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInGrid(unitCellPos, grid, unitType);

            return CheckHealable(allies, maxCount);
        }

        internal List<AllyUnit> GetAllHealableAllies(EUnitType unitType)
        {
            var allies = new List<AllyUnit>();

            void CheckHealable(IEnumerable<AllyUnit> units)
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.isActiveAndEnabled)
                    {
                        // 회복 가능 유닛이 아니라면 타겟에 추가하지 않음
                        if (unit.healthAbility.finalIsHealAble == false) continue;

                        allies.Add(unit);
                    }
                }
            }

            if ((unitType & EUnitType.Agent) != 0) CheckHealable(_agents);
            if ((unitType & EUnitType.Summon) != 0) CheckHealable(_summons);

            return allies;
        }

        private List<AllyUnit> CheckHealable(List<AllyUnit> allies, int maxCount)
        {
            var healableAllies = new List<AllyUnit>(maxCount);

            foreach (var ally in allies)
            {
                // maxCount만큼 유닛을 찾았다면
                if (healableAllies.Count >= maxCount) break;

                // 회복 가능 유닛이 아니라면 타겟에 추가하지 않음
                if (ally.GetAbility<HealthAbility>().finalIsHealAble == false) continue;

                healableAllies.Add(ally);
            }

            return healableAllies;
        }
        #endregion

        /// <summary>
        /// 범위 내에 가장 가까운 아군 유닛을 반환
        /// </summary>
        internal AllyUnit GetNearestAlly(Vector3 unitPos, float radius)
        {
            AllyUnit allyUnit = null;

            radius *= radius;
            float nearestDistance = Mathf.Infinity;

            foreach (var agent in _agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    float distance = (agent.transform.position - unitPos).sqrMagnitude;

                    if (distance < nearestDistance && distance <= radius)
                    {
                        allyUnit = agent;
                        nearestDistance = distance;
                    }
                }
            }

            foreach (var summon in _summons)
            {
                if (summon != null && summon.isActiveAndEnabled)
                {
                    float distance = (summon.transform.position - unitPos).sqrMagnitude;

                    if (distance < nearestDistance && distance <= radius)
                    {
                        allyUnit = summon;
                        nearestDistance = distance;
                    }
                }
            }

            return allyUnit;
        }
        #endregion
    }
}