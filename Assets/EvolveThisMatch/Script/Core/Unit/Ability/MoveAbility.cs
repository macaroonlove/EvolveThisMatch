using DG.Tweening;
using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public abstract class MoveAbility : ConditionAbility
    {
        [SerializeField] private bool _isLookingLeft;
        [SerializeField, ReadOnly] private float _baseMoveSpeed;
        [SerializeField, ReadOnly] private bool _isLeft;

        private BuffAbility _buffAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;
        private UnitAnimationAbility _unitAnimationAbility;
        protected float _scaleX;

        #region 계산 스탯
        protected float finalMoveSpeed
        {
            get
            {
                float result = _baseMoveSpeed;

                #region 증가·감소
                float increase = 1;

                foreach (var instance in _buffAbility.MoveIncreaseDataEffects)
                {
                    increase += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                foreach (var instance in _abnormalStatusAbility.MoveIncreaseDataEffects)
                {
                    increase += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                result *= increase;
                #endregion

                #region 상승·하락
                foreach (var instance in _buffAbility.MoveMultiplierDataEffects)
                {
                    result *= (1 + instance.effect.GetValue(unit.effectContext, instance.context));
                }
                #endregion

                return result;
            }
        }

        protected bool finalIsMoveAble
        {
            get
            {
                // 이동 불가 상태이상에 걸렸다면
                if (_abnormalStatusAbility.UnableToMoveEffects.Count > 0) return false;

                return true;
            }
        }
        #endregion

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _buffAbility = unit.GetAbility<BuffAbility>();
            _abnormalStatusAbility = unit.GetAbility<AbnormalStatusAbility>();
            _unitAnimationAbility = unit.GetAbility<UnitAnimationAbility>();

            if (unit is AgentUnit agentUnit)
            {
                _baseMoveSpeed = agentUnit.template.MoveSpeed;
            }
            else if (unit is EnemyUnit enemyUnit)
            {
                _baseMoveSpeed = enemyUnit.enemyData.template.MoveSpeed;
            }

            _scaleX = transform.GetChild(3).localScale.y;
        }

        internal override bool IsExecute()
        {
            return true;
        }

        internal override void StopAbility()
        {
            StopMoveAnimation();
            _isLeft = false;
        }

        #region 회전
        protected void FlipUnit(Vector3 direction)
        {
            bool isLeft = direction.x < 0f;

            // 방향이 변하지 않았다면 굳이 회전시키지 않기
            if (_isLeft == isLeft) return;
            _isLeft = isLeft;

            // 기본 방향을 적용
            bool needFlip = (_isLookingLeft != isLeft);

            float scaleX = needFlip ? -_scaleX : _scaleX;
            transform.GetChild(3).DOScaleX(scaleX, 0.1f);
        }
        #endregion

        protected void AttackAnimation()
        {
            _unitAnimationAbility.Attack();
        }

        protected void MoveAnimation()
        {
            _unitAnimationAbility.Move(finalMoveSpeed);
        }

        protected void StopMoveAnimation()
        {
            _unitAnimationAbility.Move(0);
        }
    }
}