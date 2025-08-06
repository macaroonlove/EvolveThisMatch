using DamageNumbersPro;
using FrameWork.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class HitAbility : AlwaysAbility
    {
        private DamageCalculateAbility _damageCalculateAbility;
        private HealthAbility _healthAbility;
        private PassiveSkillAbility _passiveSkillAbility;
        private BuffAbility _buffAbility;

        [Header("팝업 텍스트")]
        [SerializeField, Label("빗나감 팝업")] private DamageNumber _missPopup;

        [Header("FX")]
        [SerializeField, Label("피격시 실행되는 FX")] private FX _hitFX;

        #region 피해 면역 필드
        private class DamageImmunityInstance
        {
            public Coroutine coroutine;
            public int id;
            public int count;
            public float duration;
            public EDamageType damageType;

            public DamageImmunityInstance(EDamageType damageType, int count)
            {
                this.damageType = damageType;
                this.count = count;
            }

            public DamageImmunityInstance(EDamageType damageType, int id, Coroutine coroutine, float duration)
            {
                this.damageType = damageType;
                this.id = id;
                this.coroutine = coroutine;
                this.duration = duration;
            }

            public DamageImmunityInstance(EDamageType damageType, int id, Coroutine coroutine, float duration, int count)
            {
                this.damageType = damageType;
                this.id = id;
                this.coroutine = coroutine;
                this.duration = duration;
                this.count = count;
            }
        }

        private List<DamageImmunityInstance> _damageImmunities = new List<DamageImmunityInstance>();
        private int _damageImmunityIdCounter = 0;

        internal int _damageImmunityCount => _damageImmunities.Count;
        #endregion

        #region 스탯 계산
        /// <summary>
        /// 공격 대상인지
        /// </summary>
        internal bool finalTargetOfAttack
        {
            get
            {
                // 은신 상태라면 타겟이 되지 않음
                if (_buffAbility.UnableToTargetOfAttackEffects.Count > 0) return false;

                return true;
            }
        }

        private bool finalIsAvoidance
        {
            get
            {
                float avoidance = 0;

                foreach (var effect in _buffAbility.AvoidanceAdditionalDataEffects)
                {
                    avoidance += effect.Key.GetValue(effect.Value);
                }

                if (avoidance > 0)
                {
                    return Random.value < avoidance;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        internal event UnityAction onHit;

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _damageCalculateAbility = unit.GetAbility<DamageCalculateAbility>();
            _healthAbility = unit.GetAbility<HealthAbility>();
            _passiveSkillAbility = unit.GetAbility<PassiveSkillAbility>();
            _buffAbility = unit.GetAbility<BuffAbility>();
        }

        /// <summary>
        /// 유닛의 기본 공격의 경우, 피격 메서드
        /// </summary>
        internal void Hit(Unit attackedUnit)
        {
            if (finalTargetOfAttack == false) return;

            if (finalIsAvoidance)
            {
                MissPopup();
            }
            else
            {
                EDamageType damageType = attackedUnit.GetAbility<DamageCalculateAbility>().finalDamageType;

                // 피해 면역 로직
                if (UsedDamageImmunity(damageType)) return;

                ExecuteHitFX();

                int damage = _damageCalculateAbility.GetDamage(attackedUnit, damageType);
                _healthAbility.Damaged(damage, damageType, attackedUnit.id);

                onHit?.Invoke();

                foreach (var effect in _passiveSkillAbility.hitEventEffects)
                {
                    effect.Execute(unit, attackedUnit, 1);
                }
            }
        }

        /// <summary>
        /// 데미지 타입에 따른 공격
        /// </summary>
        internal void Hit(int damage, EDamageType damageType, int id = 0)
        {
            // 피해 면역 로직
            if (UsedDamageImmunity(damageType)) return;

            ExecuteHitFX();

            damage = _damageCalculateAbility.GetDamage(damage, damageType);
            _healthAbility.Damaged(damage, damageType, id);

            onHit?.Invoke();

            foreach (var effect in _passiveSkillAbility.hitEventEffects)
            {
                effect.Execute(unit, null, 1);
            }
        }

        /// <summary>
        /// 고정 피해 공격
        /// </summary>
        internal void Hit(int damage, int id = 0)
        {
            // 피해 면역 로직
            if (UsedDamageImmunity(EDamageType.TrueDamage)) return;

            ExecuteHitFX();

            _healthAbility.Damaged(damage, EDamageType.TrueDamage, id);

            onHit?.Invoke();

            foreach (var effect in _passiveSkillAbility.hitEventEffects)
            {
                effect.Execute(unit, null, 1);
            }
        }

        #region 피해 면역 로직
        /// <summary>
        /// 피해 면역이 사용되었는지
        /// </summary>
        private bool UsedDamageImmunity(EDamageType damageType)
        {
            for (int i = _damageImmunityCount - 1; i >= 0; i--)
            {
                var immunity = _damageImmunities[i];

                // 해당 피해 타입에 대한 면역이 있는지 확인
                if (immunity.damageType == damageType)
                {
                    // 횟수 기반 면역 처리
                    if (immunity.count > 0)
                    {
                        immunity.count--;
                        if (immunity.count == 0)
                        {
                            _damageImmunities.RemoveAt(i);
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 무한지속 피해 면역
        /// </summary>
        internal void AddDamageImmunity(EDamageType damageType, int count)
        {
            _damageImmunities.Add(new DamageImmunityInstance(damageType, count));
        }

        /// <summary>
        /// 지속시간 있는 피해 면역
        /// </summary>
        internal void AddDamageImmunity(EDamageType damageType, float duration)
        {
            var coroutine = StartCoroutine(CoDamageImmunity(_damageImmunityIdCounter, duration));
            _damageImmunities.Add(new DamageImmunityInstance(damageType, _damageImmunityIdCounter, coroutine, duration));
            _damageImmunityIdCounter++;
        }

        /// <summary>
        /// 지속시간 + 횟수 차감시 삭제되는 피해 면역
        /// </summary>
        internal void AddDamageImmunity(EDamageType damageType, int count, float duration)
        {
            var coroutine = StartCoroutine(CoDamageImmunity(_damageImmunityIdCounter, duration));
            _damageImmunities.Add(new DamageImmunityInstance(damageType, _damageImmunityIdCounter, coroutine, duration, count));
            _damageImmunityIdCounter++;
        }

        private IEnumerator CoDamageImmunity(int id, float duration)
        {
            yield return new WaitForSeconds(duration);

            for (int i = 0; i < _damageImmunityCount; i++)
            {
                if (_damageImmunities[i].id == id)
                {
                    _damageImmunities.RemoveAt(i);
                    break;
                }
            }
        }
        #endregion

        #region 팝업 텍스트
        private void MissPopup()
        {
            if (this == null) return;

            DamageNumber popup = _missPopup?.Spawn(transform.position);

            popup?.SetScale(0.5f);
        }
        #endregion

        #region FX
        private void ExecuteHitFX()
        {
            if (_hitFX != null)
            {
                _hitFX.Play(unit);
            }
        }
        #endregion
    }
}