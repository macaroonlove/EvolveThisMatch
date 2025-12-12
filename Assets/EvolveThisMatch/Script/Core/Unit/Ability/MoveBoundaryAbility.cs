using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 바리케이드 앞까지 이동하는 이동 어빌리티
    /// </summary>
    public class MoveBoundaryAbility : MoveAbility
    {
        [SerializeField, ReadOnly] private float _boundaryX;

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);
            
            _boundaryX = BattleManager.Instance.GetSubSystem<WaveSystem>().boundaryPoint.transform.position.x;
        }

        internal override void UpdateAbility()
        {
            if (finalIsMoveAble == false) return;

            Vector3 boundaryPosition = new Vector3(_boundaryX, transform.position.y, transform.position.z);

            Vector3 direction = (transform.position - boundaryPosition);
            float distance = direction.sqrMagnitude;

            // 회전하기
            //FlipUnit(direction.normalized);

            // 목표 위치에 도달하면
            if (distance < 0.01f)
            {
                // 공격 애니메이션
                AttackAnimation();
                return;
            }

            // 이동하기
            transform.position = Vector3.MoveTowards(transform.position, boundaryPosition, finalMoveSpeed * Time.deltaTime);

            MoveAnimation();
        }
    }
}