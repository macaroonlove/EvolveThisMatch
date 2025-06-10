using FrameWork;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UnitAnimationAbility : AlwaysAbility
    {
        private Animator _animator;

        private int _attack;
        private int _moveState;
        private int _skillID;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();

            _moveState = Animator.StringToHash("MoveState");
            _attack = Animator.StringToHash("Attack");
            _skillID = Animator.StringToHash("SkillID");
        }

        internal void Attack()
        {
            _animator.SetTrigger(_attack);
        }

        internal void Move(float speed)
        {
            _animator.SetFloat(_moveState, speed);
        }

        internal void SetSkillID(int id)
        {
            _animator.SetInteger(_skillID, id);
        }

        internal bool TrySetTrigger(int hash)
        {
            return _animator.TrySetTrigger(hash);
        }
    }
}