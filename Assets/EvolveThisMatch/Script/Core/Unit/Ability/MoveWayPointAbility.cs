using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 웨이 포인트를 경유하여 이동하는 이동 어빌리티
    /// </summary>
    public class MoveWayPointAbility : MoveAbility
    {
        private Vector3 _arrivalPoint;

        internal void InitializeArrivalPoint(Vector3 arrivalPoint)
        {
            _arrivalPoint = arrivalPoint;
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

            //#region 회전하기
            //Vector3 direction = (_arrivalPoint - transform.position).normalized;

            //// 2D 회전
            //FlipUnit(direction);
            //#endregion

            MoveAnimation();
        }
    }
}