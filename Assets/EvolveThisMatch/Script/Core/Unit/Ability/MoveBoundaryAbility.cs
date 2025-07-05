using FrameWork;
using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 경계선까지 이동하는 이동 어빌리티
    /// </summary>
    public class MoveBoundaryAbility : MoveAbility
    {
        [SerializeField, ReadOnly] private Vector3 _arrivalPoint;
        private Vector2 _boundaryPoint;
        private Vector2 _boundaryDir;
        private bool _isCached;

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            SetArrivalPoint();
        }

        private void CachedVariable()
        {
            // 경계선 위의 점을 받기
            Vector3 boundaryWorldPos = BattleManager.Instance.GetSubSystem<WaveSystem>().boundaryPoint.position;
            _boundaryPoint = new Vector2(boundaryWorldPos.x, boundaryWorldPos.y);

            // 경계선의 방향을 벡터로 만들기
            float angle = 75f * Mathf.Deg2Rad;
            _boundaryDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

            _isCached = true;
        }

        /// <summary>
        /// 도착 위치 설정
        /// </summary>
        internal void SetArrivalPoint()
        {
            if (!_isCached)
            {
                CachedVariable();
            }

            // 유닛의 위치
            Vector2 unitPos = unit.cellPos;

            // 유닛의 방향
            Vector2 unitDir = Vector2.left;

            // 시작점 차이
            Vector2 diff = _boundaryPoint - unitPos;

            // 현재 위치에서 이동할 거리
            float t = diff.Cross(_boundaryDir) / unitDir.Cross(_boundaryDir);

            // 교점 위치 = 현재 위치 + 이동할 거리 * 이동할 방향
            Vector2 crossing = unitPos + t * unitDir;

            // 적용
            _arrivalPoint = new Vector3(crossing.x, crossing.y, transform.position.z);
        }

        internal override void UpdateAbility()
        {
            if (finalIsMoveAble == false) return;

            float distance = (transform.position - _arrivalPoint).sqrMagnitude;

            // 목표 위치에 도달하면
            if (distance < 0.01f)
            {
                return;
            }

            #region 이동하기
            // 장애물이 없을 때, 직진 이동
            transform.position = Vector3.MoveTowards(transform.position, _arrivalPoint, finalMoveSpeed * Time.deltaTime);
            #endregion

            #region 회전하기
            //Vector3 direction = (_arrivalPoint - transform.position).normalized;
            //FlipUnit(direction);
            #endregion

            MoveAnimation();
        }
    }
}