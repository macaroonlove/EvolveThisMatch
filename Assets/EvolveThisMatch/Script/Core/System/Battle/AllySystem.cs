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
        [SerializeField, ReadOnly] private List<AgentBattleData> _agents = new List<AgentBattleData>();
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
                if (agent?.agentUnit?.gameObject != null)
                    Destroy(agent.agentUnit.gameObject);
            }
            _agents.Clear();

            foreach (var summon in _summons)
            {
                if (summon?.gameObject != null)
                    Destroy(summon.gameObject);
            }
            _summons.Clear();
        }

        #region 아군 유닛 등록·해제
        internal AgentBattleData Regist(AgentUnit agent, AgentTemplate agentTemplate)
        {
            var agentData = new AgentBattleData(agent, agentTemplate);
            _agents.Add(agentData);

            onRegist?.Invoke(agent);

            return agentData;
        }

        internal void Deregist(AgentBattleData agentData)
        {
            _agents.Remove(agentData);
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
        public List<AllyUnit> GetAllAllies(EUnitType unitType)
        {
            List<AllyUnit> allies = new List<AllyUnit>();

            if ((unitType & EUnitType.Agent) != 0)
            {
                foreach (var agentData in _agents)
                {
                    allies.Add(agentData.agentUnit);
                }

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
        public List<AllyUnit> GetAlliesInCircle(Vector2 unitPos, float radius, EUnitType unitType, int maxCount = int.MaxValue)
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

        private List<AllyUnit> GetAllAlliesInCircle(Vector2 unitPos, float radius, EUnitType unitType)
        {
            List<AllyUnit> allies = new List<AllyUnit>();

            radius *= radius;

            foreach (var unit in GetUnits(unitType))
            {
                if (unit != null && unit.isActiveAndEnabled)
                {
                    var distance = (unit.cellPos - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        allies.Add((unit));
                    }
                }
            }

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInCircle(Vector2 unitPos, float radius, EUnitType unitType)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            radius *= radius;

            foreach (var unit in GetUnits(unitType))
            {
                if (unit != null && unit.isActiveAndEnabled)
                {
                    var distance = (unit.cellPos - unitPos).sqrMagnitude;

                    if (distance <= radius)
                    {
                        priorityQueue.Enqueue(unit, distance);
                    }
                }
            }

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInCircle(Vector2 unitPos, float radius, EUnitType unitType, int maxCount)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            radius *= radius;

            foreach (var unit in GetUnits(unitType))
            {
                if (unit != null && unit.isActiveAndEnabled)
                {
                    var distance = (unit.cellPos - unitPos).sqrMagnitude;

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
        internal List<AllyUnit> GetAlliesInStraight(Vector2 unitPos, Vector2 targetDir, float range, float width, EUnitType unitType, int maxCount = int.MaxValue)
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

        private List<AllyUnit> GetAllAlliesInStraight(Vector2 unitPos, Vector2 targetDir, float range, float width, EUnitType unitType)
        {
            List<AllyUnit> allies = new List<AllyUnit>();

            targetDir = targetDir.normalized;
            float widthThreshold = (width * 0.5f) * (width * 0.5f);

            foreach (var unit in GetUnits(unitType))
            {
                if (unit != null && unit.isActiveAndEnabled)
                {
                    Vector2 dirVector = unit.cellPos - unitPos;

                    // 유닛을 기준으로한 정면 거리
                    float forwardDist = Vector2.Dot(targetDir, dirVector);
                    if (forwardDist < 0 || forwardDist > range) continue;

                    // 유닛을 기준으로한 측면 거리
                    float sideDist = Vector3.Cross(targetDir, dirVector).sqrMagnitude;
                    if (sideDist <= widthThreshold)
                    {
                        allies.Add(unit);
                    }
                }
            }

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInStraight(Vector2 unitPos, Vector2 targetDir, float range, float width, EUnitType unitType)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            targetDir = targetDir.normalized;
            float widthThreshold = (width * 0.5f) * (width * 0.5f);

            foreach (var unit in GetUnits(unitType))
            {
                if (unit != null && unit.isActiveAndEnabled)
                {
                    Vector2 dirVector = unit.cellPos - unitPos;

                    // 유닛을 기준으로한 정면 거리
                    float forwardDist = Vector2.Dot(targetDir, dirVector);
                    if (forwardDist < 0 || forwardDist > range) continue;

                    // 유닛을 기준으로한 측면 거리
                    float sideDist = Vector3.Cross(targetDir, dirVector).sqrMagnitude;
                    if (sideDist <= widthThreshold)
                    {
                        priorityQueue.Enqueue(unit, forwardDist + sideDist);
                    }
                }
            }

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInStraight(Vector2 unitPos, Vector2 targetDir, float range, float width, EUnitType unitType, int maxCount)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            range *= range;
            float widthThreshold = ((width * width) / 4f) * targetDir.sqrMagnitude;

            foreach (var unit in GetUnits(unitType))
            {
                if (unit != null && unit.isActiveAndEnabled)
                {
                    Vector2 dirVector = unit.cellPos - unitPos;

                    // 유닛을 기준으로한 정면 거리
                    float forwardDist = Vector2.Dot(targetDir, dirVector);
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
        internal List<AllyUnit> GetAlliesInCone(Vector2 unitPos, Vector2 targetDir, float range, int angle, EUnitType unitType, int maxCount = int.MaxValue)
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

        private List<AllyUnit> GetAllAlliesInCone(Vector2 unitPos, Vector2 targetDir, float range, int angle, EUnitType unitType)
        {
            List<AllyUnit> allies = new List<AllyUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            foreach (var unit in GetUnits(unitType))
            {
                if (unit != null && unit.isActiveAndEnabled)
                {
                    Vector2 dirVector = unit.cellPos - unitPos;
                    float distance = dirVector.sqrMagnitude;

                    if (distance <= range)
                    {
                        float dot = Vector2.Dot(targetDir, dirVector);

                        if (dot * dot >= cos * distance)
                        {
                            allies.Add(unit);
                        }
                    }
                }
            }

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInCone(Vector2 unitPos, Vector2 targetDir, float range, int angle, EUnitType unitType)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            foreach (var unit in GetUnits(unitType))
            {
                if (unit != null && unit.isActiveAndEnabled)
                {
                    Vector2 dirVector = unit.cellPos - unitPos;
                    float distance = dirVector.sqrMagnitude;

                    if (distance <= range)
                    {
                        float dot = Vector2.Dot(targetDir, dirVector);

                        if (dot * dot >= cos * distance)
                        {
                            priorityQueue.Enqueue(unit, distance);
                        }
                    }
                }
            }

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }

        private List<AllyUnit> GetSortedAlliesInCone(Vector2 unitPos, Vector2 targetDir, float range, float angle, EUnitType unitType, int maxCount)
        {
            PriorityQueue<AllyUnit> priorityQueue = new PriorityQueue<AllyUnit>();

            range *= range;
            float cos = Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
            cos *= cos;
            targetDir.Normalize();

            foreach (var unit in GetUnits(unitType))
            {
                if (unit != null && unit.isActiveAndEnabled)
                {
                    Vector2 dirVector = unit.cellPos - unitPos;
                    float distance = dirVector.sqrMagnitude;

                    if (distance <= range)
                    {
                        float dot = Vector2.Dot(targetDir, dirVector);

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

            List<AllyUnit> allies = new List<AllyUnit>(priorityQueue.Count);

            while (priorityQueue.Count > 0)
            {
                allies.Add(priorityQueue.Dequeue());
            }

            return allies;
        }
        #endregion

        #region 범위 내 공격 가능한 아군 유닛을 반환
        internal List<AllyUnit> GetAttackableAllies(Vector2 unitPos, float radius, EAttackType attackType, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInCircle(unitPos, radius, unitType);

            return CheckAttackable(allies, attackType, maxCount);
        }

        internal List<AllyUnit> GetAttackableAllies(Vector2 unitPos, Vector2 targetDir, float range, float width, EAttackType attackType, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInStraight(unitPos, targetDir, range, width, unitType);

            return CheckAttackable(allies, attackType, maxCount);
        }

        internal List<AllyUnit> GetAttackableAllies(Vector2 unitPos, Vector2 targetDir, float range, int angle, EAttackType attackType, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInCone(unitPos, targetDir, range, angle, unitType);

            return CheckAttackable(allies, attackType, maxCount);
        }

        internal List<AllyUnit> GetAllAttackableAllies(EAttackType attackType, EUnitType unitType)
        {
            var allies = new List<AllyUnit>();

            foreach (var unit in GetUnits(unitType))
            {
                if (unit != null && unit.isActiveAndEnabled)
                {
                    // 공격 대상이 아니라면 타겟에 추가하지 않음
                    if (unit.GetAbility<HitAbility>().finalTargetOfAttack == false) continue;

                    allies.Add(unit);
                }
            }

            return allies;
        }

        private List<AllyUnit> CheckAttackable(List<AllyUnit> allies, EAttackType attackType, int maxCount)
        {
            var attackableAllies = new List<AllyUnit>(maxCount);

            foreach (var ally in allies)
            {
                // maxCount만큼 유닛을 찾았다면
                if (attackableAllies.Count >= maxCount) break;

                // 공격 대상이 아니라면 타겟에 추가하지 않음
                if (ally.GetAbility<HitAbility>().finalTargetOfAttack == false) continue;

                attackableAllies.Add(ally);
            }

            return attackableAllies;
        }
        #endregion

        #region 범위 내 회복 가능한 아군 유닛을 반환
        internal List<AllyUnit> GetHealableAllies(Vector2 unitPos, float radius, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInCircle(unitPos, radius, unitType);

            return CheckHealable(allies, maxCount);
        }

        internal List<AllyUnit> GetHealableAllies(Vector2 unitPos, Vector2 targetDir, float range, float width, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInStraight(unitPos, targetDir, range, width, unitType);

            return CheckHealable(allies, maxCount);
        }

        internal List<AllyUnit> GetHealableAllies(Vector2 unitPos, Vector2 targetDir, float range, int angle, EUnitType unitType, int maxCount = int.MaxValue)
        {
            var allies = GetSortedAlliesInCone(unitPos, targetDir, range, angle, unitType);

            return CheckHealable(allies, maxCount);
        }

        internal List<AllyUnit> GetAllHealableAllies(EUnitType unitType)
        {
            var allies = new List<AllyUnit>();

            foreach (var unit in GetUnits(unitType))
            {
                if (unit != null && unit.isActiveAndEnabled)
                {
                    // 회복 가능 유닛이 아니라면 타겟에 추가하지 않음
                    if (unit.healthAbility.finalIsHealAble == false) continue;

                    allies.Add(unit);
                }
            }

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
        internal AllyUnit GetNearestAlly(Vector2 unitPos, float radius)
        {
            AllyUnit allyUnit = null;

            radius *= radius;
            float nearestDistance = Mathf.Infinity;

            foreach (var agent in GetUnits(EUnitType.Agent | EUnitType.Summon))
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    float distance = (agent.cellPos - unitPos).sqrMagnitude;

                    if (distance < nearestDistance && distance <= radius)
                    {
                        allyUnit = agent;
                        nearestDistance = distance;
                    }
                }
            }

            return allyUnit;
        }

        private IEnumerable<AllyUnit> GetUnits(EUnitType unitType)
        {
            if ((unitType & EUnitType.Agent) != 0)
            {
                foreach (var data in _agents)
                    yield return data.agentUnit;
            }

            if ((unitType & EUnitType.Summon) != 0)
            {
                foreach (var summon in _summons)
                    yield return summon;
            }
        }
        #endregion
    }
}