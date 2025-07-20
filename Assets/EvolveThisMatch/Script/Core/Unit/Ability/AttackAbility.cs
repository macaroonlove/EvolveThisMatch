using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 근접, 투사체 공격을 실행하는 어빌리티
    /// 공격력, 공격속도, 공격 사거리, 동시 공격 수, 공격 방식을 계산합니다.
    /// </summary>
    public class AttackAbility : ConditionAbility
    {
        private bool _isProjectileAttack;
        private GameObject _projectilePrefab;
        private ESpawnPoint _spawnPoint;

        private UnitAnimationAbility _unitAnimationAbility;
        private PassiveSkillAbility _passiveSkillAbility;
        private BuffAbility _buffAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;
        private FindTargetAbility _findTargetAbility;
        private EntitySpawnAbility _entitySpawnAbility;

        private int _baseATK;
        private float _baseAttackTerm;
        private float _baseAttackRange;
        private EAttackType _baseAttackType;
        private float _attackCooldown;
        private bool _isAttackActive;

        private AttackEventHandler _attackEventHandler;
        private bool _isEventAttack;

        private EAttackType _currentAttackType;
        private List<Unit> _currentTarget = new List<Unit>();

        private FX _casterFX;
        private FX _targetFX;

        #region 프로퍼티
        public int baseATK => _baseATK;
        public float baseAttackTerm => _baseAttackTerm;
        #endregion

        #region 스탯 계산
        public int finalATK
        {
            get
            {
                float result = _baseATK;

                #region 추가·차감
                foreach (var effect in _buffAbility.ATKAdditionalDataEffects.Keys)
                {
                    result += effect.value;
                }
                #endregion

                #region 증가·감소
                float increase = 1;

                foreach (var effect in _buffAbility.ATKIncreaseDataEffects.Keys)
                {
                    increase += effect.value;
                }

                result *= increase;
                #endregion

                #region 상승·하락
                foreach (var effect in _buffAbility.ATKMultiplierDataEffects.Keys)
                {
                    result *= effect.value;
                }
                #endregion

                return (int)result;
            }
        }

        public float finalAttackTerm
        {
            get
            {
                float result = _baseAttackTerm;

                #region 증가·감소
                float increase = 1;

                foreach (var effect in _buffAbility.AttackSpeedIncreaseDataEffects.Keys)
                {
                    increase += effect.value;
                }

                result *= increase;
                #endregion

                #region 상승·하락
                foreach (var effect in _buffAbility.AttackSpeedMultiplierDataEffects.Keys)
                {
                    result *= effect.value;
                }
                #endregion

                return result;
            }
        }

        private float finalAttackRange
        {
            get
            {
                float result = _baseAttackRange;

                return result;
            }
        }

        private int finalAttackCount
        {
            get
            {
                // 최대 동시 공격 수는 1명
                int result = 1;

                foreach (var effect in _buffAbility.AttackCountAdditionalDataEffects)
                {
                    result += effect.value;
                }

                return result;
            }
        }

        private EAttackType finalAttackType
        {
            get
            {
                EAttackType result = _baseAttackType;

                foreach (var effect in _buffAbility.SetAttackTypeEffects)
                {
                    result = effect.value;
                }

                return result;
            }
        }

        private bool finalIsAttackAble
        {
            get
            {
                // 공격 불가 상태이상에 걸렸다면
                if (_abnormalStatusAbility.UnableToAttackEffects.Count > 0) return false;

                return true;
            }
        }
        #endregion

        internal event UnityAction onAttack;

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _unitAnimationAbility = unit.GetAbility<UnitAnimationAbility>();
            _passiveSkillAbility = unit.GetAbility<PassiveSkillAbility>();
            _buffAbility = unit.GetAbility<BuffAbility>();
            _abnormalStatusAbility = unit.GetAbility<AbnormalStatusAbility>();
            _findTargetAbility = unit.GetAbility<FindTargetAbility>();
            _entitySpawnAbility = unit.GetAbility<EntitySpawnAbility>();

            if (unit is AgentUnit agentUnit)
            {
                _baseATK = agentUnit.template.ATK;
                _baseAttackTerm = agentUnit.template.AttackTerm;
                _baseAttackRange = agentUnit.template.AttackRange;
                _baseAttackType = agentUnit.template.AttackType;
                _casterFX = agentUnit.template.casterFX;
                _targetFX = agentUnit.template.targetFX;

                if (agentUnit.template.isProjectileAttack)
                {
                    _isProjectileAttack = true;
                    _projectilePrefab = agentUnit.template.projectilePrefab;
                    _spawnPoint = agentUnit.template.spawnPoint;
                }
            }
            else if (unit is EnemyUnit enemyUnit)
            {
                _baseATK = enemyUnit.template.ATK;
                _baseAttackTerm = enemyUnit.template.AttackTerm;
                _baseAttackRange = enemyUnit.template.AttackRange;
                _baseAttackType = EAttackType.Near;
                _casterFX = enemyUnit.template.casterFX;
                _targetFX = enemyUnit.template.targetFX;
            }

            _attackCooldown = finalAttackTerm;
            _isAttackActive = false;

            _attackEventHandler = GetComponentInChildren<AttackEventHandler>();
            if (_attackEventHandler != null)
            {
                _attackEventHandler.onAttack += OnAttackEvent;
                _isEventAttack = true;
            }
        }

        internal override void Deinitialize()
        {
            if (_attackEventHandler != null)
            {
                _attackEventHandler.onAttack -= OnAttackEvent;
                _isEventAttack = false;
            }
        }

        internal override bool IsExecute()
        {
            if (_isAttackActive) return true;

            return IsAction();
        }

        internal override void UpdateAbility()
        {
            if (_isAttackActive == false)
            {
                unit.ReleaseCurrentAbility();
            }
        }

        private bool IsAction()
        {
            // 공격 쿨타임 감소
            if (_attackCooldown > 0)
            {
                _attackCooldown -= Time.deltaTime;
                return false;
            }

            // 공격 불가 상태라면 메서드 끝내기
            if (finalIsAttackAble == false || finalAttackType == EAttackType.None) return false;
            
            _currentAttackType = finalAttackType;

            // 공격의 성공여부를 반환
            bool isAction = Action();

            // 공격이 성공했을 경우
            if (isAction)
            {
                _isAttackActive = true;

                // 쿨타임 재생
                _attackCooldown = finalAttackTerm;
            }

            return isAction;
        }

        private bool Action()
        {
            // 근거리나 원거리 유닛이라면
            if (_currentAttackType == EAttackType.Near || _currentAttackType == EAttackType.Far)
            {
                return Attack();
            }
            // 회복 유닛이라면
            else if (_currentAttackType == EAttackType.Heal)
            {
                return Heal();
            }

            return false;
        }

        #region 공격, 회복 공통 로직
        private void AttackAnimation()
        {
            _unitAnimationAbility.Attack();

            if (!_isEventAttack)
            {
                ExecuteAction();
            }
        }

        private void OnAttackEvent()
        {
            ExecuteAction();
        }

        private void ExecuteAction()
        {
            // 근거리나 원거리 유닛이라면
            if (_currentAttackType == EAttackType.Near || _currentAttackType == EAttackType.Far)
            {
                ExecuteAttack();
            }
            // 회복 유닛이라면
            else if (_currentAttackType == EAttackType.Heal)
            {
                ExecuteHeal();
            }
        }

        internal void ApplyAction(Unit target)
        {
            // 근거리나 원거리 유닛이라면
            if (_currentAttackType == EAttackType.Near || _currentAttackType == EAttackType.Far)
            {
                ApplyAttack(target);
            }
            // 회복 유닛이라면
            else if (_currentAttackType == EAttackType.Heal)
            {
                ApplyHeal(target);
            }
        }
        #endregion

        #region 공격 로직
        private bool Attack()
        {
            _currentTarget.Clear();

            var attackTargets = _findTargetAbility.FindAttackableTarget(finalAttackRange, _currentAttackType, finalAttackCount);

            if (attackTargets.Count > 0)
            {
                _currentTarget.AddRange(attackTargets);
            }

            // 공격 모션 실행
            if (_currentTarget.Count > 0)
            {
                AttackAnimation();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ExecuteAttack()
        {
            ExecuteCasterFX();

            // 투사체 공격일 경우
            if (_isProjectileAttack)
            {
                // 투사체 생성
                foreach (var attackTarget in _currentTarget)
                {
                    _entitySpawnAbility.SpawnProjectile(_projectilePrefab, _spawnPoint, attackTarget, (caster, target) => { ApplyAction(target); });
                }
            }
            // 즉시 공격일 경우
            else
            {
                foreach (var attackTarget in _currentTarget)
                {
                    ApplyAttack(attackTarget);
                }
            }

            _isAttackActive = false;
        }

        private void ApplyAttack(Unit attackTarget)
        {
            ExecuteTargetFX(attackTarget);
            
            attackTarget.GetAbility<HitAbility>().Hit(unit);

            onAttack?.Invoke();

            foreach (var effect in _passiveSkillAbility.attackEventEffects)
            {
                effect.Execute(unit, attackTarget);
            }
        }
        #endregion

        #region 회복 로직
        private bool Heal()
        {
            _currentTarget.Clear();

            var attackTargets = _findTargetAbility.FindHealableTarget(finalAttackRange, finalAttackCount);

            if (attackTargets.Count > 0)
            {
                _currentTarget.AddRange(attackTargets);
            }

            // 회복 모션 실행
            if (_currentTarget.Count > 0)
            {
                AttackAnimation();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ExecuteHeal()
        {
            ExecuteCasterFX();

            // 투사체 회복일 경우
            if (_isProjectileAttack)
            {
                // 투사체 생성
                foreach (var healTarget in _currentTarget)
                {
                    _entitySpawnAbility.SpawnProjectile(_projectilePrefab, _spawnPoint, healTarget, (caster, target) => { ApplyAction(target); });
                }
            }
            // 즉시 회복일 경우
            else
            {
                foreach (var healTarget in _currentTarget)
                {
                    ApplyHeal(healTarget);
                }
            }

            _isAttackActive = false;
        }

        private void ApplyHeal(Unit healTarget)
        {
            ExecuteTargetFX(healTarget);

            healTarget.GetAbility<HealthAbility>().Healed(finalATK, unit);

            onAttack?.Invoke();

            foreach (var effect in _passiveSkillAbility.attackEventEffects)
            {
                effect.Execute(unit, healTarget);
            }
        }
        #endregion

        #region FX
        private void ExecuteCasterFX()
        {
            if (_casterFX != null)
            {
                _casterFX.Play(unit);
            }
        }

        private void ExecuteTargetFX(Unit target)
        {
            if (_targetFX != null)
            {
                _targetFX.Play(target);
            }
        }
        #endregion
    }
}