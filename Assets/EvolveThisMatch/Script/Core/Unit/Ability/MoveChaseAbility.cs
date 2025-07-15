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

        private bool _isChaseActive;

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
                float range = enemyUnit.template.AttackRange;

                _chaseRange = range;
                _stoppingDistance = range * range;
                _chaseFailRange = _stoppingDistance * 4;
            }

            _isChaseActive = false;

            transform.GetChild(3).localScale = Vector3.one * _scaleX;
        }

        internal override bool IsExecute()
        {
            if (_targetPosition == null)
            {
                _isChaseActive = false;
            }

            if (_isChaseActive) return true;

            if (_targetPosition == null)
            {
                if (unit is AgentUnit)
                {
                    var target = BattleManager.Instance.GetSubSystem<EnemySystem>().GetNearestEnemy(unit.transform.position, _chaseRange);

                    if (target != null)
                    {
                        _targetPosition = target.transform;
                        _isChaseActive = true;
                    }
                }
                else if (unit is EnemyUnit)
                {
                    var target = BattleManager.Instance.GetSubSystem<AllySystem>().GetNearestAlly(unit.transform.position, _chaseRange);

                    if (target != null)
                    {
                        _targetPosition = target.transform;
                        _isChaseActive = true;
                    }
                }
            }
            
            // 타겟이 있다면 True, 없다면 False
            return _targetPosition != null;
        }

        internal override void UpdateAbility()
        {
            if (finalIsMoveAble == false) return;

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
            }
            #endregion

            #region 회전하기
            if (_targetPosition != null)
            {
                Vector3 direction = (_targetPosition.position - transform.position).normalized;

                // 2D 회전
                FlipUnit(direction);
            }
            #endregion

            if (_isChaseActive == false)
            {
                unit.ReleaseCurrentAbility();
            }
        }
    }
}