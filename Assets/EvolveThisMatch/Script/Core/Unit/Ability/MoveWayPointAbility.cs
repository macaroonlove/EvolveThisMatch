using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 웨이 포인트를 경유하여 이동하는 이동 어빌리티
    /// </summary>
    public class MoveWayPointAbility : MoveAbility
    {
        private Vector3 _targetPosition;
        private int _currentStep = 0;

        private List<Vector3> _wayPoint;

        internal void InitializeWayPoint(List<Vector3> wayPoint)
        {
            if (wayPoint.Count > 0)
            {
                _wayPoint = wayPoint;

                // 초기 목표 위치 지정
                _targetPosition = wayPoint[_currentStep];
            }
        }

        internal override void UpdateAbility()
        {
            if (finalIsMoveAble == false) return;
            if (_wayPoint.Count == 0) return;

            float distance = Vector2.Distance(transform.position, _targetPosition);

            // 목표 위치에 도달하면
            if (distance < 0.01f)
            {
                // 다음 목표 위치로 이동
                _currentStep = (_currentStep + 1) % _wayPoint.Count;
                _targetPosition = _wayPoint[_currentStep];
            }

            #region 이동하기
            // 장애물이 없을 때, 직진 이동
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, finalMoveSpeed * Time.deltaTime);

            // 3D 환경에서 장애물을 피해 이동
            if (_navMeshAgent != null)
            {
                _navMeshAgent.speed = finalMoveSpeed;
                _navMeshAgent.SetDestination(_targetPosition);
            }

            // 2D 환경에서 장애물을 피해 이동
            var astar = BattleManager.Instance.GetSubSystem<AStarSystem>();
            // TODO: 경로가 바뀌는 경우(장애물 생성 등)에만 Path를 재탐색하도록 수정
            var path = astar.SearchPath(transform.position, _targetPosition, AStarSystem.NodeTag.Ground);
            astar.Move(transform, path, finalMoveSpeed);
            #endregion

            #region 회전하기
            Vector3 direction = (_targetPosition - (Vector3)transform.position).normalized;

            // 2D 회전
            FlipUnit(direction);

            // 3D 회전
            //RotateUnit(direction);
            #endregion

            MoveAnimation();
        }
    }
}