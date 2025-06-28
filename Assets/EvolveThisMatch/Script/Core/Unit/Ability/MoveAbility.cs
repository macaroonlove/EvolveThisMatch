using DG.Tweening;
using FrameWork.Editor;
using UnityEngine;
using UnityEngine.AI;

namespace EvolveThisMatch.Core
{
    public class MoveAbility : ConditionAbility
    {
        [SerializeField, ReadOnly] private float _baseMoveSpeed;

        private BuffAbility _buffAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;
        private UnitAnimationAbility _unitAnimationAbility;

        #region 계산 스탯
        protected float finalMoveSpeed
        {
            get
            {
                float result = _baseMoveSpeed;

                #region 증가·감소
                float increase = 1;

                foreach (var effect in _buffAbility.MoveIncreaseDataEffects)
                {
                    increase += effect.value;
                }

                foreach (var effect in _abnormalStatusAbility.MoveIncreaseDataEffects)
                {
                    increase += effect.value;
                }

                result *= increase;
                #endregion

                #region 상승·하락
                foreach (var effect in _buffAbility.MoveMultiplierDataEffects)
                {
                    result *= effect.value;
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
                _baseMoveSpeed = enemyUnit.template.MoveSpeed;
            }
        }

        internal override bool IsExecute()
        {
            return true;
        }

        #region 회전
        private bool IsUnitLeft(Vector3 direction)
        {
            Vector3 unitRight = unit.transform.forward;
            float angle = Vector3.SignedAngle(direction, unitRight, Vector3.up);

            return angle > 0f;
        }

        protected void FlipUnit(Vector3 direction)
        {
            bool isLeft = IsUnitLeft(direction);

            float scaleX = isLeft ? -0.15f : 0.15f;
            transform.GetChild(3).DOScaleX(scaleX, 0.1f);
        }
        #endregion

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