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
        private float _attackRange;

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            if (unit is AgentUnit agentUnit)
            {
                _attackRange = agentUnit.template.AttackRange;
                _chaseRange = agentUnit.template.ChaseRange;
                _chaseFailRange = agentUnit.template.ChaseFailRange * agentUnit.template.ChaseFailRange;
                _stoppingDistance = _attackRange - 0.15f;
            }
            else if (unit is EnemyUnit enemyUnit)
            {
                _attackRange = enemyUnit.enemyData.template.AttackRange;

                _chaseRange = _attackRange;
                _stoppingDistance = _attackRange - 0.15f;
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
                    var target = BattleManager.Instance.GetSubSystem<EnemySystem>().GetNearestEnemy(unit.transform.position, _chaseRange, _attackRange);

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
            
            if (_targetPosition != null)
            {
                Vector3 targetPos = _targetPosition.position;
                Vector3 currentPos = transform.position;

                #region 이동하기
                Vector3 moveDir = targetPos - currentPos;

                // 거리 계산
                float yDiff = Mathf.Abs(moveDir.y);
                float xDiff = Mathf.Abs(moveDir.x);

                bool inAttackRangeX = xDiff <= _stoppingDistance;

                // 공격 가능하면 정지
                if (inAttackRangeX && yDiff <= 0.4f)
                {
                    _targetPosition = null;
                    StopMoveAnimation();
                }
                else
                {
                    // Y 차이가 크면 Y 보정 비중 증가
                    if (yDiff > 0.4f)
                    {
                        moveDir = new Vector3(moveDir.x * 0.3f, moveDir.y, 0f);
                    }

                    Vector3 nextPos = currentPos + moveDir.normalized * finalMoveSpeed * Time.deltaTime;
                    transform.position = MoveClamp(nextPos);

                    MoveAnimation();
                }

                // 추적 실패
                float sqrDistance = (targetPos - currentPos).sqrMagnitude;
                if (sqrDistance > _chaseFailRange)
                {
                    _targetPosition = null;
                    StopMoveAnimation();
                }
                #endregion

                #region 회전하기
                // 방향 벡터
                Vector3 direction = (targetPos - currentPos).normalized;

                // 2D 회전
                FlipUnit(direction.normalized);
                #endregion
            }
            
            if (_isChaseActive == false)
            {
                unit.ReleaseCurrentAbility();
            }
        }

        private Vector3 MoveClamp(Vector3 position)
        {
            if (position.y < 0f)
                position.y = 0f;

            return position;
        }
    }
}