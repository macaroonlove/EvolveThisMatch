using FrameWork.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 바리케이드 앞까지 이동하는 이동 어빌리티
    /// </summary>
    public class MoveBoundaryAbility : MoveAbility
    {
        [SerializeField, ReadOnly] private float _boundaryX;

        private AttackAbility _attackAbility;
        private BlockSystem _blockSystem;
        private bool _isBlocking;
        private float _attackCooldown;

        public event UnityAction onBarricadeAttack;

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _attackAbility = unit.GetAbility<AttackAbility>();
            _blockSystem = BattleManager.Instance.GetSubSystem<BlockSystem>();
            _boundaryX = BattleManager.Instance.GetSubSystem<WaveSystem>().boundaryPoint.transform.position.x;
            _isBlocking = false;
            _attackCooldown = 0;
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
                if (_isBlocking == false)
                {
                    _isBlocking = true;

                    _blockSystem.Regist(unit);
                }

                // 공격 쿨타임 감소
                if (_attackCooldown > 0)
                {
                    _attackCooldown -= Time.deltaTime;
                    return;
                }

                // 공격 애니메이션
                AttackAnimation();

                onBarricadeAttack?.Invoke();

                // 쿨타임 재생
                _attackCooldown = _attackAbility.finalAttackTerm;

                return;
            }

            if (_isBlocking)
            {
                _isBlocking = false;
                _blockSystem.Deregist(unit);
            }

            // 이동하기
            transform.position = Vector3.MoveTowards(transform.position, boundaryPosition, finalMoveSpeed * Time.deltaTime);

            MoveAnimation();
        }
    }
}