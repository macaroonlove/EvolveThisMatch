using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 유닛을 추적하는 이동 어빌리티
    /// </summary>
    public class MoveChaseAbility : MoveAbility
    {
        [SerializeField, ReadOnly] private float _chaseRange;
        [SerializeField, ReadOnly] private float _chaseFailRange;
        [SerializeField, ReadOnly] private float _stoppingDistance;
        [SerializeField, ReadOnly] private Transform _targetPosition;

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            if (unit is AgentUnit agentUnit)
            {
                _chaseRange = agentUnit.template.ChaseRange;
                _chaseFailRange = agentUnit.template.ChaseFailRange * agentUnit.template.ChaseFailRange;
                _stoppingDistance = agentUnit.template.AttackRange * agentUnit.template.AttackRange;
            }
            else if (unit is EnemyUnit enemyUnit)
            {
                _chaseRange = enemyUnit.template.ChaseRange;
                _chaseFailRange = enemyUnit.template.ChaseFailRange * enemyUnit.template.ChaseFailRange;
                _stoppingDistance = enemyUnit.template.AttackRange * enemyUnit.template.AttackRange;
            }
        }

        internal override void UpdateAbility()
        {
            if (finalIsMoveAble == false) return;

            // 목표 타겟이 없을 경우
            if (_targetPosition == null)
            {
                if (unit is AgentUnit)
                {
                    var target = BattleManager.Instance.GetSubSystem<EnemySystem>().GetNearestEnemy(unit.transform.position, _chaseRange);

                    if (target != null)
                    {
                        _targetPosition = target.transform;
                    }
                }
                else if (unit is EnemyUnit)
                {
                    var target = BattleManager.Instance.GetSubSystem<AllySystem>().GetNearestAlly(unit.transform.position, _chaseRange);

                    if (target != null)
                    {
                        _targetPosition = target.transform;
                    }
                }
            }

            #region 이동하기
            if (_targetPosition != null)
            {
                // 장애물이 없을 때, 직진 이동
                float distance = (_targetPosition.position - transform.position).sqrMagnitude;

                if (distance > _stoppingDistance)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _targetPosition.position, finalMoveSpeed * Time.deltaTime);

                    MoveAnimation();
                }
                else
                {
                    _targetPosition = null;
                    StopMoveAnimation();
                }

                if (distance > _chaseFailRange)
                {
                    _targetPosition = null;
                    StopMoveAnimation();
                }

                //// 3D 환경에서 장애물을 피해 이동
                //if (_navMeshAgent != null)
                //{
                //    _navMeshAgent.speed = finalMoveSpeed;
                //    _navMeshAgent.SetDestination(_targetPosition.position);
                //}

                //// 2D 환경에서 장애물을 피해 이동
                //var astar = BattleManager.Instance.GetSubSystem<AStarSystem>();
                //// TODO: 경로가 바뀌는 경우(장애물 생성 등)에만 Path를 재탐색하도록 수정
                //var path = astar.SearchPath(transform.position, _targetPosition.position, AStarSystem.NodeTag.Ground);
                //astar.Move(transform, path, finalMoveSpeed);
            }
            #endregion

            #region 회전하기
            if (_targetPosition != null)
            {
                Vector3 direction = (_targetPosition.position - transform.position).normalized;

                // 2D 회전
                FlipUnit(direction);

                // 3D 회전
                //RotateUnit(direction);
            }
            #endregion
        }
    }
}